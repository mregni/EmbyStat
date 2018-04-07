using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Tasks;
using EmbyStat.Common.Tasks.Interface;

namespace EmbyStat.Tasks.Tasks
{
    public class SmallSyncTask : IScheduledTask
    {
        public string Name => "Small sync with Emby";
        public string Key => "SmallEmbySync";
        public string Description => "TASKS.SMALLEMBYSYNCDESCRIPTION";
        public string Category => "Emby";
        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                progress.Report(10);
                Thread.Sleep(1000);
                progress.Report(20);
                Thread.Sleep(500);
                progress.Report(20);
                Thread.Sleep(500);
                progress.Report(22);
                Thread.Sleep(500);
                progress.Report(24);
                Thread.Sleep(500);
                progress.Report(26);
                Thread.Sleep(500);
                progress.Report(28);
                Thread.Sleep(500);
                progress.Report(40);
                Thread.Sleep(1000);
                progress.Report(60);
                Thread.Sleep(1000);
                progress.Report(75);
                Thread.Sleep(1000);
                progress.Report(80);
                Thread.Sleep(1000);
                progress.Report(85);
                Thread.Sleep(1000);
                progress.Report(90);
                Thread.Sleep(1000);
                progress.Report(100);
            }, cancellationToken);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new List<TaskTriggerInfo>
            {
                new TaskTriggerInfo{ Type = "DailyTrigger", TimeOfDayTicks = 18000000000, TaskKey = Key}
            };
        }
    }
}
