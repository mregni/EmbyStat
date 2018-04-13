using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmbyStat.Controllers.Plugin
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class PluginController : Controller
	{
		private readonly IPluginService _pluginService;

		public PluginController(IPluginService pluginService)
		{
			_pluginService = pluginService;
		}

		[HttpGet]
		public IActionResult Get()
		{
			Log.Information("Get installed plugins on Emby.");
			var result = _pluginService.GetInstalledPlugins();
			return Ok(Mapper.Map<IList<EmbyPluginViewModel>>(result));
		}
	}
}
