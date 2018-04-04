using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Tasks;

namespace EmbyStat.Tasks.Tasks
{
    public class PingEmbyTask : IScheduledTask
    {
        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            return Task.Run(() =>
            {

            }, cancellationToken);
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            throw new NotImplementedException();
        }

        public string Name => "Check Emby connection";
        public string Key => "PingEmbyServer";
        public string Description => "Ping the Emby server to check if he is still online";
        public string Category => "System";
    }
}
