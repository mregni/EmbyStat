using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Services.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmbyStat.Controllers.Tasks
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            Log.Information("Getting all background tasks.");
            var result = _taskService.GetAllTasks();
            return Ok(Mapper.Map<IList<TaskInfoViewModel>>(result));
        }
    }
}
