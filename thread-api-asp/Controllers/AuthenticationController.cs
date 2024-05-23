using Microsoft.AspNetCore.Mvc;
using thread_api_asp.Services;
using thread_api_asp.ViewModels;

namespace thread_api_asp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(IAuthenticationService authenticationService, IUserService userService)
        : ControllerBase
    {
        [HttpPost("Login")]
        public ApiResponse Login(UserLoginVm input)
        {
            string msg = authenticationService.Login(input, out TokenVm? result);
            if (msg != string.Empty) return ApiResponse.ErrorMessage(msg);
            return ApiResponse.Ok(result);
        }

        [HttpPost("Register")]
        public ApiResponse Register(UserInsertVm input)
        {
            string msg = userService.InsertUser(input, out object? userVm);
            if (msg != string.Empty) return ApiResponse.ErrorMessage(msg);
            return ApiResponse.Ok(userVm);
        }

        [HttpPost("RenewToken")]
        public ApiResponse RenewToken(TokenVm input)
        {
            string msg = authenticationService.RenewToken(input, out TokenVm? token);
            if (msg != string.Empty) return ApiResponse.ErrorMessage(msg);
            return ApiResponse.Ok(token);
        }

    }
}