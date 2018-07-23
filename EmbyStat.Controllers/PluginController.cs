using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Controllers.ViewModels.Emby;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmbyStat.Controllers
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
			var result = _pluginService.GetInstalledPlugins();
			return Ok(Mapper.Map<IList<EmbyPluginViewModel>>(result));
		}
    }
}
