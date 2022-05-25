using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Languages.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Controllers.Settings;

[Produces("application/json")]
[Route("api/[controller]")]
public class SettingsController : Controller
{
    private readonly IConfigurationService _configurationService;
    private readonly ILanguageService _languageService;
    private readonly IMapper _mapper;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(IConfigurationService configurationService,
        ILanguageService languageService, IMapper mapper, ILogger<SettingsController> logger)
    {
        _languageService = languageService;
        _configurationService = configurationService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var config = _configurationService.Get();
        
        var settingsViewModel = _mapper.Map<ConfigViewModel>(config);
        return Ok(settingsViewModel);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UserConfigViewModel userConfigViewModel)
    {
        if (userConfigViewModel == null)
        {
            _logger.LogInformation("Settings object was NULL while calling the PUT API.");
            return BadRequest();
        }

        var userConfig = _mapper.Map<UserConfig>(userConfigViewModel);
        await _configurationService.UpdateUserConfiguration(userConfig);
        
        var config = _configurationService.Get();
        return Ok(config);
    }

    [HttpGet]
    [Route("languages")]
    public async Task<IActionResult> GetList()
    {
        var result = await _languageService.GetLanguages();
        return Ok(_mapper.Map<IList<LanguageViewModel>>(result));
    }
}