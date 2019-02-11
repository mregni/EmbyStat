using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Controllers.ViewModels.Configuration;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmbyStat.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class SettingsController : Controller
	{
        private readonly ISettingsService _settingsService;
	    private readonly IMapper _mapper;

	    public SettingsController(ISettingsService settingsService, IMapper mapper)
	    {
            _settingsService = settingsService;
	        _mapper = mapper;
	    }

	    [HttpGet]
	    public IActionResult Get()
	    {
	        var settings = _settingsService.GetUserSettings();
	        if (!settings.WizardFinished)
	        {
                Log.Information($"{Constants.LogPrefix.ServerApi}\tStarting wizard for user.");
	        }

            var settingsViewModel = _mapper.Map<FullSettingsViewModel>(settings);
            settingsViewModel.Version = _settingsService.GetAppSettings().Version;

            return Ok(settingsViewModel);
	    }

	    [HttpPut]
	    public async Task<IActionResult> Update([FromBody] FullSettingsViewModel userSettings)
	    {
	        var settings = _mapper.Map<UserSettings>(userSettings);
            settings = await _settingsService.SaveUserSettings(settings);
            var settingsViewModel = _mapper.Map<FullSettingsViewModel>(settings);

            settingsViewModel.Version = _settingsService.GetAppSettings().Version;
            return Ok(settingsViewModel);
        }
    }
}
