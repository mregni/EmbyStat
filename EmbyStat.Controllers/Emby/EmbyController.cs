using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.Configuration;
using EmbyStat.Services.Config;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Emby.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Controllers.Emby
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class EmbyController : Controller
	{
		private readonly ILogger<EmbyController> _logger;
		private readonly IPluginService _embyService;

		public EmbyController(IPluginService embyService, ILogger<EmbyController> logger)
		{
			_logger = logger;
			_embyService = embyService;
		}

		[HttpPost]
		[Route("generatetoken")]
		public async Task<IActionResult> GenerateToken([FromBody]EmbyLoginViewModel login)
		{
			_logger.LogInformation("Get emby token for certain login credentials.");
			var result = await _embyService.GetEmbyToken(Mapper.Map<EmbyLogin>(login));
			return Ok(Mapper.Map<EmbyTokenViewModel>(result));
		}

		[HttpPost]
		[Route("firesmallembysync")]
		public IActionResult FireSmallEmbySync()
		{
			_logger.LogInformation("Sync basic Emby server info.");
			_embyService.FireSmallSyncEmbyServerInfo();
			return Ok();
		}

		[HttpGet]
		[Route("getserverinfo")]
		public IActionResult GetServerInfo()
		{
			_logger.LogInformation("Get Emby server info.");
			var result = _embyService.GetServerInfo();
			return Ok(Mapper.Map<ServerInfoViewModel>(result));
		}

		[HttpGet]
		[Route("searchemby")]
		public IActionResult SearchEmby()
		{
			_logger.LogInformation("Searching for an Emby server in the network and returning the IP address.");
			var result = _embyService.SearchEmby();
			if (!string.IsNullOrWhiteSpace(result.Address))
			{
				_logger.LogInformation("Emby server found at: " + result.Address);
			}
			return Ok(Mapper.Map<EmbyUdpBroadcastViewModel>(result));
		}
	}
}
