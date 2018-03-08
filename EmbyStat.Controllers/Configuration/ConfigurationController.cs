using AutoMapper;
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
		    var configuration = _configurationService.GetServerSettings();
            return Ok(Mapper.Map<ConfigurationViewModel>(configuration));
        }

	    [HttpPut]
	    public IActionResult Update()
	    {
		    return Ok();
	    }
    }
}
