using Microsoft.AspNetCore.Mvc;
using thread_api_asp.Services;
using thread_api_asp.ViewModels;

namespace thread_api_asp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(
        IAuthenticationService authenticationService,
        IUserService userService
    )
        : ControllerBase
    {
        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public Result Login(UserLoginVm input)
        {
            string msg = authenticationService.Login(input, out var output);
            return output == null ? Result.ErrorMessage(msg) : Result.OkMessage(output, msg);
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public Result Register(UserInsertVm input)
        {
            string msg = userService.InsertUser(input);
            if (msg != string.Empty) return Result.ErrorMessage(msg);
            return Result.Ok();
        }

        /// <summary>
        /// Khởi tạo lại token cho người dùng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("RefreshToken")]
        public Result RefreshToken(TokenVm input)
        {
            try
            {
                var token = authenticationService.RefreshToken(input);
                return Result.Ok(token);
            }
            catch (Exception e) { return Result.ErrorMessage(e.Message); }
        }

    }
}