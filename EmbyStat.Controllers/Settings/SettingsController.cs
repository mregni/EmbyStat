using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Logging;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Settings;

[Produces("application/json")]
[Route("api/[controller]")]
public class SettingsController : Controller
{
    private readonly ISettingsService _settingsService;
    private readonly ILanguageService _languageService;
    private readonly IMapper _mapper;
    private readonly Logger _logger;

    public SettingsController(ISettingsService settingsService,
        ILanguageService languageService, IMapper mapper)
    {
        _languageService = languageService;
        _settingsService = settingsService;
        _mapper = mapper;
        _logger = LogFactory.CreateLoggerForType(typeof(SettingsController), "SETTINGS");
    }

    [HttpGet]
    public IActionResult Get()
    {
        var settings = _settingsService.GetUserSettings();
        var appSettings = _settingsService.GetAppSettings();
        var settingsViewModel = _mapper.Map<FullSettingsViewModel>(settings);
        settingsViewModel.Version = appSettings.Version;
        settingsViewModel.NoUpdates = appSettings.NoUpdates;
        settingsViewModel.DataDir = appSettings.Dirs.Data;
        settingsViewModel.ConfigDir = appSettings.Dirs.Config;
        settingsViewModel.LogDir = appSettings.Dirs.Logs;

        return Ok(settingsViewModel);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] FullSettingsViewModel userSettings)
    {
        if (userSettings == null)
        {
            _logger.Info("Settings object was NULL while calling the PUT API.");
            return BadRequest();
        }

        var settings = _mapper.Map<UserSettings>(userSettings);

        settings = await _settingsService.SaveUserSettingsAsync(settings);
        var settingsViewModel = _mapper.Map<FullSettingsViewModel>(settings);
        settingsViewModel.Version = _settingsService.GetAppSettings().Version;

        return Ok(settingsViewModel);
    }

    [HttpGet]
    [Route("languages")]
    public async Task<IActionResult> GetList()
    {
        var result = await _languageService.GetLanguages();
        return Ok(_mapper.Map<IList<LanguageViewModel>>(result));
    }
}