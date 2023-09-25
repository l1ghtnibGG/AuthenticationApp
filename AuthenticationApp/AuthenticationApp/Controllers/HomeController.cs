using AuthenticationApp.Models;
using AuthenticationApp.Models.Repo;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthenticationApp.Models.RegistrationsModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _repository;

        public HomeController(IUserRepository repository, ILogger<HomeController> logger)
        {
            _repository = repository ?? throw new ArgumentException(nameof(repository));
            _logger = logger;
        }

        public IActionResult Registration() => View();

        public IActionResult Error(string message) => View("Error", message);

        [Authorize]
        public IActionResult UserPanel(string id)
        {
            var user = CheckId(id);
            
            if (user == null)
                return RedirectToAction("Error", new {  message = 
                    "User wasn't found" });
            
            if (!user.Status)
                return RedirectToAction("Error", new {  message = 
                    "User was blocked" });
            
            return View(_repository.Users); 
        }

        private User? CheckId(string id)
        {
            try
            {
                var userId = Guid.Parse(id);
                var user = _repository.Users.First(x => x.Id == userId);

                return user;
            }
            catch (FormatException ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message, this);
                 RedirectToAction("Error", new {  message = 
                    "Wrong user id" });
                 return null;
            }
            catch (ArgumentNullException ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message, this);
                 RedirectToAction("Error", new {  message = 
                    "User wasn't found" });
                 return null;
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message, this);
                RedirectToAction("Error", new {  message = 
                    "Wrong user id" });
                return null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registration([FromForm]UserRegistrationModel user)
        {
            if (ModelState.IsValid)
            {
                if (_repository.Users.FirstOrDefault(x => x.Email == user.Email) == null)
                {
                    return await CreateUser(new User
                    {
                        Email = user.Email,
                        Name = user.Name,
                        Password = user.Password
                    });
                }

                return RedirectToAction("Error", new { message = "User already exist. Try again or log in." });
            }
            
            return View(user);
        }

        private async Task<IActionResult> CreateUser(User user)
        {
            var createdUser = _repository.AddUser(user);

            await UserSingIn(createdUser);

            return RedirectToAction("UserPanel", new {id = createdUser.Id.ToString()});
        }
        
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);
            
            if (user == null)
                return RedirectToAction("Error", new { message = "User not found. Try again or registrate." });

            if (!user.Status)
                return RedirectToAction("Error",
                    new { message = "You're was blocked. Create new account or say someone to unblock you." });

            await UserSingIn(user);

            return RedirectToAction("UserPanel", "Home" , new {id = user.Id.ToString()});
        }
        
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Registration");
        }

        private async Task UserSingIn(User user)
        {
            var claims = GenerateClaims(user);

            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity));
        }
        
        [HttpPost("Block")]
        public IActionResult Block(List<string> userId)
        {
            var users = UserIdToGuid(userId);
            
            if(users == null)
                return RedirectToAction("Error", new { message = "User not found." });

            foreach (var user in users)
            {
                if (!_repository.BlockUser(user))
                    return RedirectToAction("Error", new { message = "User not found." });
            }
            
            if(User.Identity.IsAuthenticated)
                return RedirectToAction("UserPanel", new {id = User.Claims.First().Value});

            return RedirectToAction("Login");
        }
        
        [HttpPost("UnBlock")]
        public IActionResult Unblock(List<string> userId)
        {
            var users = UserIdToGuid(userId);
            
            if(users == null)
                return RedirectToAction("Error", new { message = "User not found." });

            foreach (var user in users)
            {
                if (!_repository.UnblockUser(user))
                    return RedirectToAction("Error", new { message = "User not found." });
            }
            
            if(User.Identity.IsAuthenticated)
                return RedirectToAction("UserPanel", new {id = User.Claims.First().Value});

            return RedirectToAction("Login");
        }
        
        [HttpPost("Delete")]
        public IActionResult Delete(List<string> userId)
        {
            var users = UserIdToGuid(userId);
            
            if(users == null)
                return RedirectToAction("Error", new { message = "User not found." });

            foreach (var user in users)
            {
                if (!_repository.DeleteUser(user))
                    return RedirectToAction("Error", new { message = "User not found." });
            }
            
            if(User.Identity.IsAuthenticated)
                return RedirectToAction("UserPanel", new {id = User.Claims.First().Value});

            return RedirectToAction("Login");;
        }

        private Claim[] GenerateClaims(User user) =>
            new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

        private List<Guid>? UserIdToGuid(List<string> users)
        {
            var list = new List<Guid>();
            try
            {
                list.AddRange(users.Select(Guid.Parse));

                return list;
            }
            catch (FormatException e)
            {
                _logger.Log(LogLevel.Critical, e.Message, this);
                return null;
            }
        }

        private User? Authenticate(UserLogin userLogin) =>
            _repository.Users.FirstOrDefault(x => x.Email == userLogin.Email
                                                  && x.Password == userLogin.Password);
    }
}
