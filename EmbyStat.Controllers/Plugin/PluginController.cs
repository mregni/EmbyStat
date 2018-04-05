using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Services.Plugin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Controllers.Plugin
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class PluginController : Controller
	{
		private readonly ILogger<PluginController> _logger;
		private readonly IPluginService _pluginService;

		public PluginController(IPluginService pluginService, ILogger<PluginController> logger)
		{
			_logger = logger;
			_pluginService = pluginService;
		}

		[HttpGet]
		public IActionResult Get()
		{
			_logger.LogInformation("Get installed plugins on Emby.");
			var result = _pluginService.GetInstalledPlugins();
			return Ok(Mapper.Map<IList<EmbyPluginViewModel>>(result));
		}
	}
}
