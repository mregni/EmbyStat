using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Services.HangFire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Controllers.Task
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class TaskController : Controller
	{
		private readonly IHangFireService _hangFireService;
		private readonly ILogger<TaskController> _logger;

		public TaskController(IHangFireService hangFireService, ILogger<TaskController> logger)
		{
			_hangFireService = hangFireService;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Get()
		{
			_logger.LogInformation("Getting background tasks from HangFire.");
			var tasks = _hangFireService.GetTasks();
			return Ok(Mapper.Map<IList<BackgroundTaskViewModel>>(tasks));
		}

		[HttpPost]
		[Route("fire")]
		public IActionResult FireTask([FromBody] IdViewModel data)
		{
			if (!string.IsNullOrWhiteSpace(data.Id))
			{
				_hangFireService.FireTask(data.Id);
			}

			return Ok();
		}
	}
}
