using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Configuration;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EmbyStat.Controllers.Job;

[Produces("application/json")]
[Route("api/[controller]")]
public class JobController : Controller
{
    private readonly IMapper _mapper;
    private readonly IJobService _jobService;
    private readonly IHubHelper _hubHelper;
    private readonly IJobInitializer _jobInitializer;
    private readonly Config _config;

    public JobController(IMapper mapper, IJobService jobService, IHubHelper hubHelper,
        IJobInitializer jobInitializer, IOptions<Config> options)
    {
        _mapper = mapper;
        _jobService = jobService;
        _hubHelper = hubHelper;
        _jobInitializer = jobInitializer;
        _config = options.Value;
    }

    [HttpGet]
    [Route("")]
    public IActionResult GetAll()
    {
        var jobs = _jobService.GetAll();

        if (!_config.SystemConfig.CanUpdate)
        {
            jobs = jobs.Where(x => x.Id != Constants.JobIds.CheckUpdateId);
        }

        return Ok(_mapper.Map<IList<JobViewModel>>(jobs));
    }

    [HttpGet]
    [Route("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var job = _jobService.GetById(id);
        if (job == null)
        {
            return NotFound();
        }
        
        return Ok(_mapper.Map<JobViewModel>(job));
    }

    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateTrigger(Guid id, string cron)
    {
        if (await _jobService.UpdateTrigger(id, cron))
        {
            _jobInitializer.UpdateTrigger(id, cron, _config.SystemConfig.CanUpdate);
            return NoContent();
        }

        return NotFound();
    }

    [HttpPost]
    [Route("fire/{id:guid}")]
    public async Task<IActionResult> FireJob(Guid id)
    {
        var job = _jobService.GetById(id);

        if (job == null)
        {
            return new NotFoundResult();
        }
        
        await Task.Run(() => { RecurringJob.Trigger(job.Id.ToString()); });
        await _hubHelper.BroadcastJobLog("JOBS", $"{job.Title} job queued", ProgressLogType.Information);
        return Ok();
    }
}