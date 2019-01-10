using System;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using Serilog;
using TaskInfo = EmbyStat.Common.Models.Tasks.TaskInfo;

namespace EmbyStat.Tasks.Tasks
{
    public abstract class TaskConnector
    {
        private readonly IHubHelper _hubHelper;

        protected TaskConnector(IHubHelper hubHelper)
        {
            _hubHelper = hubHelper;
        }

        public abstract Guid Id { get; }
        public abstract string Name { get; }
        public abstract string Key { get; }
        public abstract string Description { get; }
        public abstract string Category { get; }

        public async void LogTaskProgress(double progress, TaskState state = TaskState.Running)
        {
            var info = new TaskInfo
            {
                Id = Id.ToString(),
                Category = Category,
                CurrentProgressPercentage = progress,
                Description = Description,
                Name = Name,
                State = state
            };
            await _hubHelper.BroadcastTaskProgress(info);
        }

        public void LogInformation(string prefix, string text)
        {
            Log.Information($"{prefix}\t{text}");
            SendUpdateToFront(text, ProgressLogType.Normal);
        }

        public void LogWarning(string prefix, string text)
        {
            Log.Warning($"{prefix}\t{text}");
            SendUpdateToFront(text, ProgressLogType.Warning);
        }

        public void LogError(string prefix, string text)
        {
            Log.Error($"{prefix}\t{text}");
            SendUpdateToFront(text, ProgressLogType.Error);
        }

        private async void SendUpdateToFront(string text, ProgressLogType type)
        {
            var log = new TaskLog { Type = type, Value = text };
            await _hubHelper.BroadCastTaskLog(log);
        }
    }
}