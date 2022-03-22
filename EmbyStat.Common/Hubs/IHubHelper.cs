using System.Threading.Tasks;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Common.Hubs
{
    public interface IHubHelper
    {
        Task BroadcastJobProgress(JobProgress info);
        Task BroadcastJobLog(string jobName, string message, ProgressLogType type);
        Task BroadcastEmbyConnectionStatus(int missedPings);
        Task BroadcastUpdateState(bool isRunning);
        Task BroadcastUpdateFinished(bool successful);
        Task BroadcastResetLogLine(string line);
        Task BroadcastResetFinished();
    }
}
