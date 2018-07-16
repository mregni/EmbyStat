using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.ViewModels.Language;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LanguageController : Controller
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        [Route("getlist")]
        public IActionResult GetList()
        {
            var result = _languageService.GetLanguages();
            return Ok(Mapper.Map<IList<LanguageViewModel>>(result));
        }
    }
}
