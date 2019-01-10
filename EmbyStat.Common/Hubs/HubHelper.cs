using System.Threading.Tasks;
using EmbyStat.Common.Models.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace EmbyStat.Common.Hubs
{
    public class HubHelper : IHubHelper
    {
        private readonly IHubContext<TaskHub> _taskHubContext;
        private string LogUpdateCommand => "LogUpdateReceived";
        private string TaskUpdateReceivedCommand => "TaskUpdateReceived";

        public HubHelper(IHubContext<TaskHub> taskHubContext)
        {
            _taskHubContext = taskHubContext;
        }

        public async Task BroadcastTaskProgress(TaskInfo info)
        {
            await _taskHubContext.Clients.All.SendAsync(LogUpdateCommand, info);
        }

        public async Task BroadCastTaskLog(TaskLog log)
        {
            await _taskHubContext.Clients.All.SendAsync(TaskUpdateReceivedCommand, log);
        }
    }
}