using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Language
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LanguageController : Controller
    {
        private readonly ILanguageService _languageService;
        private readonly IMapper _mapper;

        public LanguageController(ILanguageService languageService, IMapper mapper)
        {
            _languageService = languageService;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Route("getlist")]
        public IActionResult GetList()
        {
            var result = _languageService.GetLanguages();
            return Ok(_mapper.Map<IList<LanguageViewModel>>(result));
        }
    }
}
