using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
	    [HttpGet]
        public IActionResult Get(string name)
        {
            return Ok(new
            {
                Name = name
            });
        }

        [HttpPost]
        public IActionResult Post()
        {
            return Ok(new
            {
                Name = "Mikhael"
            });
        }
    }
}
