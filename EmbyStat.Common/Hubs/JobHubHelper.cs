using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using Microsoft.AspNetCore.SignalR;

namespace EmbyStat.Common.Hubs
{
    public class JobHubHelper : IJobHubHelper
    {
        private readonly IHubContext<JobHub> _jobHubContext;
        private string JobReportLogMethod => "job-report-log";
        private string JobReportProgressMethod => "job-report-progress";
        private string EmbyConnectionStatusMethod => "emby-connection-status";

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
    }
}