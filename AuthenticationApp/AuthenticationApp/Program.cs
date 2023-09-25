using AuthenticationApp.Models;
using AuthenticationApp.Models.Data;
using AuthenticationApp.Models.Repo;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthenticationApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("AuthAppConnection");
            builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/LogIn";
                    options.LogoutPath = "/Home/LogOut";
                    options.AccessDeniedPath = "/Home/Error";
                    options.ReturnUrlParameter = "ReturnUrl";
                });

            builder.Services.AddScoped<IUserRepository, EFUserRepository>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => 
            { 
                endpoints.MapControllerRoute("login", 
                    "LogIn", 
                    new { controller = "Home", action = "Login" });
                endpoints.MapControllerRoute("notFound",
                    "Error",
                    new { controller = "Home", action = "Error" });
                endpoints.MapControllerRoute("userPanel",
                    "UserPanel/{id}",
                    new { controller = "Home", action = "UserPanel", id = 0 });
                endpoints.MapControllerRoute("registration",
                    "SingUp",
                    new { controller = "Home", action = "Registration" });
                endpoints.MapControllerRoute("default",
                    pattern: "{controller=Home}/{action=Registration}");
            });

            SeedData.EnsureUsers(app);
            app.Run();
        }
    }
}