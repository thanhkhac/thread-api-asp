using thread_api_asp.Entities;
using thread_api_asp.ViewModels;

namespace thread_api_asp.Repository
{
    public interface IUserRepository
    {
        public User? GetUserByUsernameAndPassword(UserLoginVm input);
        public User? GetUserByUsername(string username);
        public int Add(User input);
    }

    public class UserRepository(ThreadsContext context) : IUserRepository
    {
        public User? GetUserByUsernameAndPassword(UserLoginVm input)
        {
            var result = (
                from user in context.Users
                where
                    user.Username.Equals(input.UserName)
                    && user.Password.Equals(input.Password)
                select user).FirstOrDefault();
            return result;
        }

        public User? GetUserByUsername(string username)
        {
            var result = (
                from user in context.Users
                where
                    user.Username.Equals(username)
                select user).FirstOrDefault();
            return result;
        }

        public int Add(User input)
        {
            context.Users.Add(input);
            return context.SaveChanges();
        }

    }
}