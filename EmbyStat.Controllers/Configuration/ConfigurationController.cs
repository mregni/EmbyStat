using AutoMapper;
using EmbyStat.Common.Exceptions;
using EmbyStat.Services.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Controllers.Configuration
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
			_logger.LogInformation("Getting all server configuration from database.");
		    var configuration = _configurationService.GetServerSettings();
            return Ok(Mapper.Map<ConfigurationViewModel>(configuration));
        }

	    [HttpPut]
	    public IActionResult Update()
	    {
		    return Ok();
	    }

	    [HttpGet]
		[Route("searchemby")]
	    public IActionResult SearchEmby()
	    {
_logger.LogInformation("Searching for an Emby server in the network and returning the IP address.");
		    var result = _configurationService.SearchEmby();
		    return Ok(Mapper.Map<EmbyUdpBroadcastViewModel>(result));
	    }
    }
}
