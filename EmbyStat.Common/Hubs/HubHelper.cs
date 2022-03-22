using System;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using Microsoft.AspNetCore.SignalR;

namespace EmbyStat.Common.Hubs
{
    public class HubHelper : IHubHelper
    {
        private readonly IHubContext<EmbyStatHub> _hubContext;
        private static string JobReportLogMethod => "JobReportLog";
        private static string JobReportProgressMethod => "JobReportProgress";
        private static string EmbyConnectionStatusMethod => "MediaServerConnectionState";
        private static string UpdateIsRunningMethod => "UpdateState";
        private static string UpdateFinishedMethod => "UpdateFinished";
        private static string ResetDatabaseFinishedMethod => "ResetDatabaseFinished";
        private static string ResetDatabaseLogLineMethod => "ResetDatabaseLogLine";

        public HubHelper(IHubContext<EmbyStatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastJobProgress(JobProgress info)
        {
            await _hubContext.Clients.All.SendAsync(JobReportProgressMethod, info);
        }

        public async Task BroadcastJobLog(string jobName, string message, ProgressLogType type)
        {
            var log = new JobLog { Type = type, Value = message, JobName = jobName, DateTimeUtc = DateTime.UtcNow};
            await _hubContext.Clients.All.SendAsync(JobReportLogMethod, log);
        }

        public async Task BroadcastEmbyConnectionStatus(int missedPings)
        {
            await _hubContext.Clients.All.SendAsync(EmbyConnectionStatusMethod, missedPings);
        }

        public async Task BroadcastUpdateState(bool isRunning)
        {
            await _hubContext.Clients.All.SendAsync(UpdateIsRunningMethod, isRunning);
        }

        public async Task BroadcastUpdateFinished(bool successful)
        {
            await _hubContext.Clients.All.SendAsync(UpdateFinishedMethod, successful ? 1 : 2);
        }

        public async Task BroadcastResetLogLine(string line)
        {
            await _hubContext.Clients.All.SendAsync(ResetDatabaseLogLineMethod, new { Line = line});
        }
        public async Task BroadcastResetFinished()
        {
            await _hubContext.Clients.All.SendAsync(ResetDatabaseFinishedMethod);
        }
    }
}