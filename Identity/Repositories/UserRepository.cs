using Identity.Entities;

namespace Identity.Repositories
{
    public interface IUserRepository
    {
        public User? GetUserByUserName(string userName);
    }

    public class UserRepository(MyIdentityDbContext context) : IUserRepository
    {

        public User? GetUserByUserName(string userName) => context.Users.SingleOrDefault(x => x.UserName == userName);

    }
}