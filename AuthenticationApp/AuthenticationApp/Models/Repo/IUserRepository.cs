using System.Security.Claims;

namespace AuthenticationApp.Models.Repo
{
    public interface IUserRepository
    {
        public IQueryable<User> Users { get; }

        public User AddUser(User user);

        public bool BlockUser(Guid id);

        public bool UnblockUser(Guid id);

        public bool DeleteUser(Guid id);
    }
}
