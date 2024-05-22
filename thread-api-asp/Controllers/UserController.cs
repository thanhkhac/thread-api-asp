
using Microsoft.AspNetCore.Mvc;

using thread_api_asp.Services;


namespace thread_api_asp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {



    }
}
