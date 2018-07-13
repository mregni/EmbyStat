using System;
using System.Collections.Generic;
using EmbyStat.Common;
using EmbyStat.Common.Extentions;
using EmbyStat.Controllers.ViewModels.Configuration;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class ConfigurationController : Controller
	{
		private readonly IConfigurationService _configurationService;
		private readonly ILogger<ConfigurationController> _logger;

		public ConfigurationController(IConfigurationService configurationService, ILogger<ConfigurationController> logger)
		{
			_configurationService = configurationService;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Get()
		{
			_logger.LogInformation("Getting server configuration from database.");
			var configuration = _configurationService.GetServerSettings();
			return Ok(ConvertToViewModel(configuration));
		}

		[HttpPut]
		public IActionResult Update([FromBody] ConfigurationViewModel configuration)
		{
		    _logger.LogInformation("Updating the new server configuration.");
		    var config = ToDictionary(configuration);
		    _configurationService.SaveServerSettings(config);

		    config = _configurationService.GetServerSettings();
		    return Ok(ConvertToViewModel(config));
        }

	    private static ConfigurationViewModel ConvertToViewModel(IReadOnlyDictionary<string, string> configuration)
	    {
	        return new ConfigurationViewModel
	        {
	            AccessToken = configuration[Constants.Configuration.AccessToken],
	            EmbyServerAddress = configuration[Constants.Configuration.EmbyServerAddress],
	            EmbyUserId = configuration[Constants.Configuration.EmbyUserId],
	            EmbyUserName = configuration[Constants.Configuration.EmbyUserName],
	            Language = configuration[Constants.Configuration.Language],
	            ServerName = configuration[Constants.Configuration.ServerName],
	            ToShortMovie = Convert.ToInt32(configuration[Constants.Configuration.ToShortMovie]),
	            Username = configuration[Constants.Configuration.UserName],
	            WizardFinished = configuration[Constants.Configuration.WizardFinished].ToBoolean()
	        };
	    }

	    private static Dictionary<string, string> ToDictionary(ConfigurationViewModel configuration)
	    {
            return new Dictionary<string, string>
            {
                { Constants.Configuration.AccessToken, configuration.AccessToken },
                { Constants.Configuration.EmbyServerAddress, configuration.EmbyServerAddress },
                { Constants.Configuration.EmbyUserId, configuration.EmbyUserId },
                { Constants.Configuration.EmbyUserName, configuration.EmbyUserName },
                { Constants.Configuration.Language, configuration.Language },
                { Constants.Configuration.ServerName, configuration.ServerName },
                { Constants.Configuration.ToShortMovie, configuration.ToShortMovie.ToString() },
                { Constants.Configuration.UserName, configuration.Username },
                { Constants.Configuration.WizardFinished, configuration.WizardFinished.ToString() },
            };
	    }
	}
}
