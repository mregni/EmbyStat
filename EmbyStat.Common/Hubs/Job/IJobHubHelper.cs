using System.Threading.Tasks;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Common.Hubs.Job
{
    public interface IJobHubHelper
    {
        Task BroadcastJobProgress(JobProgress info);
        Task BroadCastJobLog(string prefix, string message, ProgressLogType type);
        Task BroadcastEmbyConnectionStatus(int missedPings);
        Task BroadcastUpdateState(bool isRunning);
    }
}
