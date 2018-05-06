using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DummyController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("TEST OK");
        }
    }
}
