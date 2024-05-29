using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [Authorize]
        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok("Ok");
        }
    }
}