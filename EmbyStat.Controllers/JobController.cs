using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Controllers.ViewModels.Task;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPingEmbyJob _pingEmbyJob;
        private readonly IJobService _jobService;
        private readonly IJobHubHelper _jobHubHelper;
        private readonly ICheckUpdateJob _checkUpdateJob;
        private readonly IMediaSyncJob _mediaSyncJob;
        private readonly ISmallSyncJob _smallSyncJob;
        private readonly IDatabaseCleanupJob _databaseCleanupJob;
        private string LogPrefix => Constants.LogPrefix.JobController;

        public JobController(IMapper mapper, IPingEmbyJob pingEmbyJob, IJobService jobService, 
            IJobHubHelper jobHubHelper, ICheckUpdateJob checkUpdateJob, IMediaSyncJob mediaSyncJob, 
            ISmallSyncJob smallSyncJob, IDatabaseCleanupJob databaseCleanupJob)
        {
            _mapper = mapper;
            _pingEmbyJob = pingEmbyJob;
            _jobService = jobService;
            _jobHubHelper = jobHubHelper;
            _checkUpdateJob = checkUpdateJob;
            _mediaSyncJob = mediaSyncJob;
            _smallSyncJob = smallSyncJob;
            _databaseCleanupJob = databaseCleanupJob;
        }
        
        [HttpGet]
        [Route("")]
        public IActionResult GetAll()
        {
            var jobs = _jobService.GetAll();
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

        [HttpPost]
        [Route("ping/fire")]
        public async Task<IActionResult> FireTask()
        {
            await Task.Run(() => { RecurringJob.Trigger(Constants.JobIds.PingEmbyId.ToString()); });
            await _jobHubHelper.BroadCastJobLog(LogPrefix, $"New {Constants.LogPrefix.PingEmbyJob} job queued", ProgressLogType.Information);
            return Ok();
        }

        [HttpPost]
        [Route("checkupdate/fire")]
        public async Task<IActionResult> FireCheckUpdate()
        {
            BackgroundJob.Enqueue(() => _checkUpdateJob.Execute());
            await _jobHubHelper.BroadCastJobLog(LogPrefix, $"New {Constants.LogPrefix.CheckUpdateJob} job queued", ProgressLogType.Information);
            return Ok();
        }

        [HttpPost]
        [Route("mediasync/fire")]
        public async Task<IActionResult> FireMediaSync()
        {
            BackgroundJob.Enqueue(() => _mediaSyncJob.Execute());
            await _jobHubHelper.BroadCastJobLog(LogPrefix, $"New {Constants.LogPrefix.MediaSyncJob} job queued", ProgressLogType.Information);
            return Ok();
        }

        [HttpPost]
        [Route("smallsync/fire")]
        public async Task<IActionResult> FireSmallSync()
        {
            BackgroundJob.Enqueue(() => _smallSyncJob.Execute());
            await _jobHubHelper.BroadCastJobLog(LogPrefix, $"New {Constants.LogPrefix.SmallEmbySyncJob} job queued", ProgressLogType.Information);
            return Ok();
        }

        [HttpPost]
        [Route("databasecleanup/fire")]
        public async Task<IActionResult> FireDatabaseCleanup()
        {
            await Task.Run(() => { RecurringJob.Trigger(Constants.JobIds.DatabaseCleanupId.ToString()); });
            await _jobHubHelper.BroadCastJobLog(LogPrefix, $"New {Constants.LogPrefix.DatabaseCleanupJob} job queued", ProgressLogType.Information);
            return Ok();
        }
    }
}