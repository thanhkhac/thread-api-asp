using Identity.Services;
using Identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(IAuthenticationService authenticationService)
    {
        [HttpPost("SignUp")]
        public ApiResponse SignUp(SignUpVm input)
        {
            var status = authenticationService.SignUp(input, out IdentityResult output);
            return status.IsOk ? ApiResponse.OkMessage(output, status.Message) : ApiResponse.ErrorMessage(output, status.Message);
        }
        
        [HttpPost("Login")]
        public ApiResponse Login(LoginVm input)
        {
            var status = authenticationService.Login(input, out TokenVm? output);
            return status.IsOk ? ApiResponse.OkMessage(output, status.Message) : ApiResponse.ErrorMessage(output, status.Message);
        }
        
        [HttpPost("Refresh")]
        public ApiResponse Refresh(RefreshTokenVm input)
        {
            var status = authenticationService.RefreshToken(input, out TokenVm? output);
            return status.IsOk ? ApiResponse.OkMessage(output, status.Message) : ApiResponse.ErrorMessage(output, status.Message);
        }
    }
}