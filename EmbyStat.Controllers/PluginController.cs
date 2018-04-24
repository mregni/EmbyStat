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

		public PluginController()
		{
			//_pluginService = pluginService;
		}

		[HttpGet]
		public IActionResult Get()
		{
			Log.Information("Get installed plugins on Emby.");
			var result = _pluginService.GetInstalledPlugins();
			return Ok(Mapper.Map<IList<EmbyPluginViewModel>>(result));
		}

	    [HttpGet]
	    public IActionResult GetDummy()
	    {
	        return Ok();
	    }
	}
}
