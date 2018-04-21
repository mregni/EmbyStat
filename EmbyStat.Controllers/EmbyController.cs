using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.ViewModels.Emby;
using EmbyStat.Controllers.ViewModels.Server;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class EmbyController : Controller
	{
		private readonly ILogger<EmbyController> _logger;
		private readonly IEmbyService _embyService;

		public EmbyController(IEmbyService embyService, ILogger<EmbyController> logger)
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
			var drives = _embyService.GetLocalDrives();

			var serverInfo = Mapper.Map<ServerInfoViewModel>(result);
			serverInfo.Drives = Mapper.Map<IList<DriveViewModel>>(drives).ToList();

			return Ok(serverInfo);
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
