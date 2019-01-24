using System;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using Microsoft.AspNetCore.SignalR;

namespace EmbyStat.Common.Hubs.Job
{
    public class JobHubHelper : IJobHubHelper
    {
        private readonly IHubContext<JobHub> _jobHubContext;
        private static string JobReportLogMethod => "job-report-log";
        private static string JobReportProgressMethod => "job-report-progress";
        private static string EmbyConnectionStatusMethod => "emby-connection-state";
        private static string UpdateIsRunningMethod => "update-state";

        public JobHubHelper(IHubContext<JobHub> jobHubContext)
        {
            _jobHubContext = jobHubContext;
        }

        public async Task BroadcastJobProgress(JobProgress info)
        {
            await _jobHubContext.Clients.All.SendAsync(JobReportProgressMethod, info);
        }

        public async Task BroadCastJobLog(string prefix, string message, ProgressLogType type)
        {
            var text = $"{prefix} => {message}";
            var log = new JobLog { Type = type, Value = text };

            await _jobHubContext.Clients.All.SendAsync(JobReportLogMethod, log);
        }

        public async Task BroadcastEmbyConnectionStatus(int missedPings)
        {
            await _jobHubContext.Clients.All.SendAsync(EmbyConnectionStatusMethod, missedPings);
        }

        public async Task BroadcastUpdateState(bool isRunning)
        {
            await _jobHubContext.Clients.All.SendAsync(UpdateIsRunningMethod, isRunning);
        }
    }
}