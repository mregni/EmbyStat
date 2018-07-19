using System;
using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Extentions;
using EmbyStat.Controllers.ViewModels.Configuration;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EmbyStat.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class ConfigurationController : Controller
	{
        private readonly IConfigurationService _configurationService;

		public ConfigurationController(IConfigurationService configurationService)
		{
			_configurationService = configurationService;
		}

	    [HttpGet]
	    public IActionResult Get()
	    {
	        Log.Information($"{ Constants.LogPrefix.ServerApi}\tGetting server configuration from database.");
	        var configuration = _configurationService.GetServerSettings();
	        if (!configuration.WizardFinished)
	        {
                Log.Information($"{Constants.LogPrefix.ServerApi}\tStarting wizard for user.");
	        }
	        return Ok(Mapper.Map<ConfigurationViewModel>(configuration));
	    }

	    [HttpPut]
	    public IActionResult Update([FromBody] ConfigurationViewModel configuration)
	    {
	        Log.Information($"{Constants.LogPrefix.ServerApi}\tUpdating the new server configuration.");
	        var config = Mapper.Map<Common.Models.Configuration>(configuration);
	        _configurationService.SaveServerSettings(config);

	        config = _configurationService.GetServerSettings();
	        return Ok(Mapper.Map<ConfigurationViewModel>(config));
        }
    }
}
