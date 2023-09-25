using Microsoft.EntityFrameworkCore;

namespace AuthenticationApp.Models.Data
{
    public static class SeedData
    {
        public static void EnsureUsers(IApplicationBuilder app)
        {
            var context = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<UserDbContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Name = "Vlad",
                        Email = "vlad123@mail.ru",
                        Password = "1234",
                        CreatedDate = DateTime.Now.AddDays(-1),
                        LastLogInDate = DateTime.Now,
                        Status = true
                    },
                    new User
                    {
                        Name = "Vika",
                        Email = "vika123@mail.ru",
                        Password = "1234",
                        CreatedDate = DateTime.Now.AddDays(-15),
                        LastLogInDate = DateTime.Now,
                        Status = true
                    },
                    new User
                    {
                        Name = "Igor",
                        Email = "igor123@gmail.com",
                        Password = "1234",
                        CreatedDate = DateTime.Now,
                        LastLogInDate = DateTime.Now,
                        Status = true
                    },
                    new User
                    {
                        Name = "Misha",
                        Email = "misha123@gmail.com",
                        Password = "1234",
                        CreatedDate = DateTime.Now.AddDays(-10),
                        LastLogInDate = DateTime.Now.AddDays(-1),
                        Status = false
                    },
                    new User
                    {
                        Name = "Natasha",
                        Email = "natasha123@gmail.com",
                        Password = "1234",
                        CreatedDate = DateTime.Now.AddDays(-1),
                        LastLogInDate = DateTime.Now,
                        Status = false
                    }
                );
                context.SaveChanges();
            }   
        }
    }

}
