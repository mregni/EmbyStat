using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using EmbyStat.Controllers.ViewModels.Activity;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ActivityController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IActivityService _activityService;

        public ActivityController(IMapper mapper, IActivityService activityService)
        {
            _mapper = mapper;
            _activityService = activityService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var activities = _activityService.GetLastUserActivities();
            return Ok(_mapper.Map<IList<UserActivityViewModel>>(activities));
        }
    }
}
