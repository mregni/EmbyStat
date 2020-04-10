using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;

namespace EmbyStat.Jobs.Jobs.Sync
{
    [DisableConcurrentExecution(60)]
    public class SmallSyncJob : BaseJob, ISmallSyncJob
    {
        private readonly IMediaServerService _mediaServerService;

        public SmallSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService, 
            IMediaServerService mediaServerService) 
            : base(hubHelper, jobRepository, settingsService, typeof(SmallSyncJob), Constants.LogPrefix.SmallEmbySyncJob)
        {
            _mediaServerService = mediaServerService;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.SmallSyncId;
        public override string JobPrefix => Constants.LogPrefix.SmallEmbySyncJob;
        public override string Title { get; }

        public override async Task RunJobAsync()
        {
            _mediaServerService.GetAndProcessServerInfo();
            await LogInformation("Server info downloaded");
            await LogProgress(35);

            _mediaServerService.GetAndProcessPluginInfo();
            await LogInformation("Server plugins downloaded");
            await LogProgress(55);

            _mediaServerService.GetAndProcessUsers();
            await LogInformation("Server users downloaded");
            await LogProgress(80);

            _mediaServerService.GetAndProcessDevices();
            await LogInformation("Server devices downloaded");
        }
    }
}