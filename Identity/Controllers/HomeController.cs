using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [HttpGet("Hello")]
        [Authorize]
        public IActionResult Get()
        {
            return Ok("Ok");
        }
    }
}