using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [HttpGet("hello")]
        [Authorize(Policy = "hello")]
        public IActionResult Get()
        {
            return Ok("Ok");
        }
    }
}