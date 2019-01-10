using System.Threading.Tasks;
using EmbyStat.Common.Models.Tasks;

namespace EmbyStat.Common.Hubs
{
    public interface IHubHelper
    {
        Task BroadcastTaskProgress(TaskInfo info);
        Task BroadCastTaskLog(TaskLog log);
    }
}
