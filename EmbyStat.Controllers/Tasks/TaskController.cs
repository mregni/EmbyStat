using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Common.Tasks;
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

        [HttpPut]
        [Route("triggers")]
        public IActionResult UpdateTriggers([FromBody] TaskInfoViewModel task)
        {
            _taskService.UpdateTriggers(Mapper.Map<TaskInfo>(task));
            var result = _taskService.GetAllTasks();
            return Ok(Mapper.Map<IList<TaskInfoViewModel>>(result));
        }

        [HttpPost]
        [Route("fire/{id}")]
        public IActionResult FireTask(string id)
        {
            _taskService.FireTask(id);
            return Ok();
        }
    }
}
