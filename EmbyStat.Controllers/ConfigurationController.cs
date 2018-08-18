using System;
using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models.Entities;
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
	    private readonly IMapper _mapper;

	    public ConfigurationController(IConfigurationService configurationService, IMapper mapper)
	    {
	        _configurationService = configurationService;
	        _mapper = mapper;
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
	        return Ok(_mapper.Map<ConfigurationViewModel>(configuration));
	    }

	    [HttpPut]
	    public IActionResult Update([FromBody] ConfigurationViewModel configuration)
	    {
	        Log.Information($"{Constants.LogPrefix.ServerApi}\tUpdating the new server configuration.");
	        var config = _mapper.Map<Configuration>(configuration);
	        _configurationService.SaveServerSettings(config);

	        config = _configurationService.GetServerSettings();
	        return Ok(_mapper.Map<ConfigurationViewModel>(config));
        }
    }
}
