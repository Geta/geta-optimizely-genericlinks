using Microsoft.AspNetCore.Mvc;

namespace AlloyMvcTemplates.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}