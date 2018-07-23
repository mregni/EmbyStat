using AutoMapper;
using EmbyStat.Controllers.ViewModels.About;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var about = _aboutService.GetAbout();
            return Ok(Mapper.Map<AboutViewModel>(about));
        }
    }
}
