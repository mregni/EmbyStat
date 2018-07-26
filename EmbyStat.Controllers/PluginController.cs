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
	    private readonly IMapper _mapper;

	    public PluginController(IPluginService pluginService, IMapper mapper)
	    {
	        _pluginService = pluginService;
	        _mapper = mapper;
	    }

		[HttpGet]
		public IActionResult Get()
		{
			var result = _pluginService.GetInstalledPlugins();
			return Ok(_mapper.Map<IList<EmbyPluginViewModel>>(result));
		}
    }
}
