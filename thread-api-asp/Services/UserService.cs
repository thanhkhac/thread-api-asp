using System.Dynamic;
using thread_api_asp.Commons;
using thread_api_asp.Models;
using thread_api_asp.ViewModels;

namespace thread_api_asp.Services
{
    public interface IUserService
    {
        public string GetUserByUsernameAndPassword(UserLoginVm input, out object? result);
        public string InsertUser(UserInsertVm input, out object? result);
    }

    public class UserService(ThreadsContext context, IRoleService roleService) : IUserService
    {
        private const string MessUsernameIsExist = "Username này đã tồn tại";

        public string GetUserByUsernameAndPassword(UserLoginVm input, out object? result)
        {
            result = (
                from user in context.Users
                where
                    user.Username.Equals(input.UserName)
                    && user.Password.Equals(input.Password)
                select new UserVm
                {
                    Id = user.Id,
                    Username = user.Username
                }).SingleOrDefault();
            return string.Empty;
        }

        private string GetUserByUsername(string username, out object? result)
        {
            result = (
                from user in context.Users
                where
                    user.Username.Equals(username)
                select new UserVm
                {
                    Id = user.Id,
                    Username = user.Username
                }).SingleOrDefault();
            return string.Empty;
        }

        public string InsertUser(UserInsertVm input, out object? result)
        {
            result = new ExpandoObject();
            GetUserByUsername(input.UserName, out object? existUser);
            if (existUser != null) return MessUsernameIsExist;
            User user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = input.UserName,
                Password = input.Password,
                Roles = roleService.GetDefaultRoles()
            };
            context.Users.Add(user);
            return DbHelper.SaveChangeHandleError(context);
        }
    }
}