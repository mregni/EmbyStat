using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace EmbyStat.Controllers.Settings
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class SettingsController : Controller
	{
        private readonly ISettingsService _settingsService;
        private readonly ILanguageService _languageService;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IMapper _mapper;

        public SettingsController(ISettingsService settingsService, IStatisticsRepository statisticsRepository, ILanguageService languageService, IMapper mapper)
        {
            _languageService = languageService;
            _settingsService = settingsService;
            _statisticsRepository = statisticsRepository;
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
            if (!(useSettings.MovieLibraryTypes.All(configuration.MovieLibraryTypes.Contains) &&
                  useSettings.MovieLibraryTypes.Count == configuration.MovieLibraryTypes.Count))
            {
                _statisticsRepository.MarkMovieTypesAsInvalid();
            }

            if (!(useSettings.ShowLibraryTypes.All(configuration.ShowLibraryTypes.Contains) &&
                  useSettings.ShowLibraryTypes.Count == configuration.ShowLibraryTypes.Count))
            {
                _statisticsRepository.MarkShowTypesAsInvalid();
            }
        }
    }
}
