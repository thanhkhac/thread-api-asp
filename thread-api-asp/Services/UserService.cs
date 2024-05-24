using thread_api_asp.Entities;
using thread_api_asp.Errors;
using thread_api_asp.Repository;
using thread_api_asp.ViewModels;

namespace thread_api_asp.Services
{
    public interface IUserService
    {
        public string InsertUser(UserInsertVm input);
    }

    public class UserService(IUserRepository userRepository, IRoleService roleService) : IUserService
    {

        public string InsertUser(UserInsertVm input)
        {
            try
            {
                var existUser = userRepository.GetUserByUsername(input.UserName);
                if (existUser != null) return "Username này đã tồn tại";
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = input.UserName,
                    Password = input.Password,
                    Roles = roleService.GetDefaultRoles()
                };
                userRepository.Add(user);
                return string.Empty;
            }
            catch (Exception e)
            {
                //Log lỗi tại đây
                return ErrorConstants.CommonError;
            }
        }
    }
}