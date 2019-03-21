using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
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
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IMapper _mapper;
        private readonly Logger _logger;

        public SettingsController(ISettingsService settingsService, IStatisticsRepository statisticsRepository, IMapper mapper)
        {
            _settingsService = settingsService;
            _statisticsRepository = statisticsRepository;
            _mapper = mapper;
            _logger = LogManager.GetCurrentClassLogger();
        }

	    [HttpGet]
	    public IActionResult Get()
        {
	        var settings = _settingsService.GetUserSettings();
	        if (!settings.WizardFinished)
	        {
                _logger.Info($"{Constants.LogPrefix.ServerApi}\tStarting wizard for user.");
	        }

            var settingsViewModel = _mapper.Map<FullSettingsViewModel>(settings);
            settingsViewModel.Version = _settingsService.GetAppSettings().Version;

            return Ok(settingsViewModel);
	    }

	    [HttpPut]
	    public async Task<IActionResult> Update([FromBody] FullSettingsViewModel userSettings)
	    {
	        var settings = _mapper.Map<UserSettings>(userSettings);
            MarkStatisticsAsInvalidIfNeeded(settings);
            settings = await _settingsService.SaveUserSettings(settings);
            var settingsViewModel = _mapper.Map<FullSettingsViewModel>(settings);

            settingsViewModel.Version = _settingsService.GetAppSettings().Version;
            return Ok(settingsViewModel);
        }

        private void MarkStatisticsAsInvalidIfNeeded(UserSettings configuration)
        {
            var useSettings = _settingsService.GetUserSettings();
            if (!(useSettings.MovieCollectionTypes.All(configuration.MovieCollectionTypes.Contains) &&
                  useSettings.MovieCollectionTypes.Count == configuration.MovieCollectionTypes.Count))
            {
                _statisticsRepository.MarkMovieTypesAsInvalid();
            }

            if (!(useSettings.ShowCollectionTypes.All(configuration.ShowCollectionTypes.Contains) &&
                  useSettings.ShowCollectionTypes.Count == configuration.ShowCollectionTypes.Count))
            {
                _statisticsRepository.MarkShowTypesAsInvalid();
            }
        }
    }
}
