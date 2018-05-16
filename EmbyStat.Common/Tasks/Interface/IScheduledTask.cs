using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmbyStat.Common.Tasks.Interface
{
    public interface IScheduledTask
    {
        string Name { get; }
        string Key { get; }
        string Description { get; }
        string Category { get; }
        Task Execute(CancellationToken cancellationToken, IProgress<double> progress, IProgress<string> logProgress);
        IEnumerable<TaskTriggerInfo> GetDefaultTriggers();
        void LogInformation(string log);
        void LogWarning(string log);
    }
}
