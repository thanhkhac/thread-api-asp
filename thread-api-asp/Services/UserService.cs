using thread_api_asp.Entities;
using thread_api_asp.Errors;
using thread_api_asp.Repository;
using thread_api_asp.ViewModels;

namespace thread_api_asp.Services
{
    public interface IUserService
    {
        public ServiceResult InsertUser(UserInsertVm input);
    }

    public class UserService(IUserRepository userRepository, IRoleService roleService) : IUserService
    {

        public ServiceResult InsertUser(UserInsertVm input)
        {
            try
            {
                var existUser = userRepository.GetUserByUsername(input.UserName);
                if (existUser != null) ServiceResult.Error("Username này đã tồn tại");
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = input.UserName,
                    Password = input.Password,
                    Roles = roleService.GetDefaultRoles()
                };
                userRepository.Add(user);
                return ServiceResult.Ok("Tạo tài khoản thành công");
            }
            catch (Exception e)
            {
                //Log lỗi tại đây
                return ServiceResult.Error(ErrorConstants.CommonError);
            }
        }
    }
}