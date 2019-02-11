using AutoMapper;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.About
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;
        private readonly IMapper _mapper;

        public AboutController(IAboutService aboutService, IMapper mapper)
        {
            _aboutService = aboutService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var about = _aboutService.GetAbout();
            return Ok(_mapper.Map<AboutViewModel>(about));
        }
    }
}
