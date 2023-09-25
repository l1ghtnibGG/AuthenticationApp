using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationApp.Models.Repo
{
    public class EFUserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public EFUserRepository(UserDbContext context)
        {
            _context = context;
        }

        public IQueryable<User> Users => _context.Users;

        public User AddUser(User user)
        {
            user.CreatedDate = DateTime.Now;
            user.LastLogInDate = DateTime.Now;
            user.Status = true;

            _context.Add(user);
            _context.SaveChanges();

            return user;
        }

        public bool DeleteUser(Guid id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            _context.SaveChanges();

            return true;
        }

        public bool BlockUser(Guid id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return false;

            user.Status = false;
            _context.Users.Update(user);
            _context.SaveChanges();

            return true;
        }
        
        public bool UnblockUser(Guid id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return false;

            user.Status = true;
            _context.Users.Update(user);
            _context.SaveChanges();
            
            return true;
        }
    }
}
