using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Controllers.ViewModels.Emby;
using EmbyStat.Controllers.ViewModels.Server;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmbyStat.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class EmbyController : Controller
	{
		private readonly IEmbyService _embyService;
	    private readonly IMapper _mapper;

	    public EmbyController(IEmbyService embyService, IMapper mapper)
	    {
	        _embyService = embyService;
	        _mapper = mapper;
	    }
	    
        [HttpPost]
		[Route("generatetoken")]
		public async Task<IActionResult> GenerateToken([FromBody]EmbyLoginViewModel login)
		{
			Log.Information($"{Constants.LogPrefix.ServerApi}\tGet emby token for certain login credentials.");
			var result = await _embyService.GetEmbyToken(_mapper.Map<EmbyLogin>(login));
			return Ok(_mapper.Map<EmbyTokenViewModel>(result));
		}

		[HttpGet]
		[Route("getserverinfo")]
		public async Task<IActionResult> GetServerInfo()
		{
		    Log.Information($"{Constants.LogPrefix.ServerApi}\tGet Emby server info.");
			var result = await _embyService.GetServerInfo();
			var drives = _embyService.GetLocalDrives();

			var serverInfo = _mapper.Map<ServerInfoViewModel>(result);
			serverInfo.Drives = _mapper.Map<IList<DriveViewModel>>(drives).ToList();

			return Ok(serverInfo);
		}

		[HttpGet]
		[Route("searchemby")]
		public IActionResult SearchEmby()
		{
		    Log.Information($"{Constants.LogPrefix.ServerApi}\tSearching for an Emby server in the network and returning the IP address.");
			var result = _embyService.SearchEmby();
			if (!string.IsNullOrWhiteSpace(result.Address))
			{
			    Log.Information($"{Constants.LogPrefix.ServerApi}\tEmby server found at: " + result.Address);
			}
			return Ok(_mapper.Map<EmbyUdpBroadcastViewModel>(result));
		}

	    [HttpGet]
	    [Route("getembystatus")]
	    public IActionResult GetEmbyStatus()
	    {
	        var result = _embyService.GetEmbyStatus();
	        return Ok(_mapper.Map<EmbyStatusViewModel>(result));
	    }
    }
}
