using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Settings
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class SettingsController : Controller
	{
        private readonly ISettingsService _settingsService;
        private readonly ILanguageService _languageService;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IMediaServerService _mediaServerService;
        private readonly IMapper _mapper;

        public SettingsController(ISettingsService settingsService, IStatisticsRepository statisticsRepository, ILanguageService languageService, IMapper mapper, IMediaServerService mediaServerService)
        {
            _languageService = languageService;
            _settingsService = settingsService;
            _statisticsRepository = statisticsRepository;
            _mediaServerService = mediaServerService;
            _mapper = mapper;
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
            var settings = _mapper.Map<UserSettings>(userSettings);

            MarkStatisticsAsInvalidIfNeeded(settings);
            settings = await _settingsService.SaveUserSettingsAsync(settings);
            var settingsViewModel = _mapper.Map<FullSettingsViewModel>(settings);
            settingsViewModel.Version = _settingsService.GetAppSettings().Version;

            //TODO, check if user checked the new to implement checkbox to reset the database.
            
            return Ok(settingsViewModel);
        }

        [HttpGet]
        [Route("languages")]
        public IActionResult GetList()
        {
            var result = _languageService.GetLanguages();
            return Ok(_mapper.Map<IList<LanguageViewModel>>(result));
        }

        private void MarkStatisticsAsInvalidIfNeeded(UserSettings configuration)
        {
            var useSettings = _settingsService.GetUserSettings();
            if (!(useSettings.MovieLibraries.All(configuration.MovieLibraries.Contains) &&
                  useSettings.MovieLibraries.Count == configuration.MovieLibraries.Count))
            {
                _statisticsRepository.MarkMovieTypesAsInvalid();
            }

            if (!(useSettings.ShowLibraries.All(configuration.ShowLibraries.Contains) &&
                  useSettings.ShowLibraries.Count == configuration.ShowLibraries.Count))
            {
                _statisticsRepository.MarkShowTypesAsInvalid();
            }
        }
    }
}
