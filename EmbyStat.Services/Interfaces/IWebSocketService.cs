using System.Threading;
using System.Threading.Tasks;

namespace EmbyStat.Services.Interfaces
{
    public interface IWebSocketService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        bool IsWebSocketConnectionOpen();
    }
}
