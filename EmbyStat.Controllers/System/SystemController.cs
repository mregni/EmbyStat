using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.Emby;
using EmbyStat.Services.Emby.Models;
using EmbyStat.Services.System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Controllers.System
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class SystemController : Controller
	{
		private readonly ILogger<SystemController> _logger;
		private readonly ISystemService _systemService;

		public SystemController(ISystemService systemService, ILogger<SystemController> logger)
		{
			_logger = logger;
			_systemService = systemService;
		}
		
		[HttpPost]
	    [Route("shutdown")]
	    public IActionResult Shutdown()
	    {
		    _logger.LogInformation("Shutdown server on users request.");
		    _logger.LogInformation("Sweet dreams!!");
			
		    _systemService.StartShutdownJob();

			return Ok();
	    }
	}
}
