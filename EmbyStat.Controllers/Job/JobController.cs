using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Jobs;
using EmbyStat.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Job
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IJobService _jobService;
        private readonly IJobHubHelper _jobHubHelper;
        private readonly IJobInitializer _jobInitializer;
        private readonly ISettingsService _settingsService;

        public JobController(IMapper mapper, IJobService jobService, IJobHubHelper jobHubHelper,
            IJobInitializer jobInitializer, ISettingsService settingsService)
        {
            _mapper = mapper;
            _jobService = jobService;
            _jobHubHelper = jobHubHelper;
            _jobInitializer = jobInitializer;
            _settingsService = settingsService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var settings = _settingsService.GetAppSettings();
            var jobs = _jobService.GetAll();

            if (settings.NoUpdates)
            {
                jobs = jobs.Where(x => x.Id != Constants.JobIds.CheckUpdateId);
            }

            return Ok(_mapper.Map<IList<JobViewModel>>(jobs));
        }

        [HttpGet]
        [Route("{id}")]
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
        [Route("{id}")]
        public IActionResult UpdateTrigger(Guid id, string cron)
        {
            if (_jobService.UpdateTrigger(id, cron))
            {
                var settings = _settingsService.GetAppSettings();
                _jobInitializer.UpdateTrigger(id, cron, settings.NoUpdates);
                return NoContent();
            }

            return NotFound();
        }

        [HttpPost]
        [Route("fire/{id}")]
        public async Task<IActionResult> FireJob(Guid id)
        {
            var job = _jobService.GetById(id);
            if (job == null)
            {
                return NotFound();
            }

            await Task.Run(() => { RecurringJob.Trigger(job.Id.ToString()); });
            await _jobHubHelper.BroadCastJobLog("JOB CONTROLLER", $"{GetJobTitle(job.Title)} job queued", ProgressLogType.Information);
            return Ok();
        }

        [HttpGet]
        [Route("mediasync")]
        public IActionResult GetMediaSyncJob()
        {
            return Get(Constants.JobIds.MediaSyncId);
        }

        private string GetJobTitle(string key)
        {
            switch (key)
            {
                case "DATABASECLEANUPTITLE": return Constants.LogPrefix.DatabaseCleanupJob;
                case "CHECKUPDATETITLE": return Constants.LogPrefix.CheckUpdateJob;
                case "SMALLEMBYSYNCTITLE": return Constants.LogPrefix.SmallEmbySyncJob;
                case "MEDIASYNCTITLE": return Constants.LogPrefix.MediaSyncJob;
                case "PINGEMBYSERVERTITLE": return Constants.LogPrefix.PingEmbyJob;
                default: return string.Empty;
            }
        }
    }
}