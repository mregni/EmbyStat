using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Common.Tasks;
using EmbyStat.Controllers.ViewModels.Task;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var result = _taskService.GetAllTasks();
            return Ok(_mapper.Map<IList<TaskInfoViewModel>>(result));
        }

        [HttpPut]
        [Route("triggers")]
        public IActionResult UpdateTriggers([FromBody] TaskInfoViewModel task)
        {
            _taskService.UpdateTriggers(Mapper.Map<TaskInfo>(task));
            var result = _taskService.GetAllTasks();
            return Ok(_mapper.Map<IList<TaskInfoViewModel>>(result));
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