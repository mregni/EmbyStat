using System.Collections.Generic;
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

        [HttpGet]
        [Route("state")]
        public IActionResult GetStates()
        {
            Log.Information("Getting task states;");
            var result = _taskService.GetStates();
            return Ok(Mapper.Map<IList<TaskStatusViewModel>>(result));
        }

        [HttpGet]
        [Route("state/{id}")]
        public IActionResult GetState(string id)
        {
            Log.Information($"Getting task states for id: {id}");
            var result = _taskService.GetStateByTaskId(id);
            return Ok(Mapper.Map<TaskStatusViewModel>(result));
        }

        [HttpPost]
        [Route("trigger")]
        public IActionResult AddTrigger([FromBody] string trigger)
        {
            return NotFound();
        }

        [HttpDelete]
        [Route("trigger")]
        public IActionResult DeleteTrigger(string id)
        {
            return NotFound();
        }

        [HttpPost]
        [Route("fire/{id}")]
        public IActionResult FireTask(string id)
        {
            return NotFound();
        }
    }
}
