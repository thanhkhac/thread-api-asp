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

        [HttpPost("Login")]
        public ApiResponse Login(UserLoginVm input)
        {
            var status = authenticationService.Login(input, out var output);
            return status.IsOk ? ApiResponse.OkMessage(output, status.Message) : ApiResponse.ErrorMessage(output, status.Message);
        }

        [HttpPost("Register")]
        public ApiResponse Register(UserInsertVm input)
        {
            var status = userService.InsertUser(input);
            return status.IsOk ? ApiResponse.OkMessage(status.Message) : ApiResponse.ErrorMessage(status.Message);
        }

        [HttpPost("RefreshToken")]
        public ApiResponse RefreshToken(TokenLoginVm input)
        {
            var status = authenticationService.RefreshToken(input, out TokenVm? output);
            return status.IsOk ? ApiResponse.OkMessage(output, status.Message) : ApiResponse.ErrorMessage(output, status.Message);
        }

    }
}