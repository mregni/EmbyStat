using System;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Hubs;
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

        public SmallSyncJob(IHubHelper hubHelper, IJobRepository jobRepository, ISettingsService settingsService, 
            IMediaServerService mediaServerService) 
            : base(hubHelper, jobRepository, settingsService, typeof(SmallSyncJob), Constants.LogPrefix.SmallMediaServerSyncJob)
        {
            _mediaServerService = mediaServerService;
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.SmallSyncId;
        public override string JobPrefix => Constants.LogPrefix.SmallMediaServerSyncJob;
        public override string Title { get; }

        public override async Task RunJobAsync()
        {
            await _mediaServerService.GetAndProcessServerInfo();
            await LogInformation("Server info downloaded");
            await LogProgress(35);

            await _mediaServerService.GetAndProcessPluginInfo();
            await LogInformation("Server plugins downloaded");
            await LogProgress(55);

            await _mediaServerService.GetAndProcessUsers();
            await LogInformation("Server users downloaded");
            await LogProgress(80);

            await _mediaServerService.GetAndProcessDevices();
            await LogInformation("Server devices downloaded");
            await LogProgress(90);

            await _mediaServerService.GetAndProcessLibraries();
            await LogInformation("Server libraries downloaded");
        }
    }
}