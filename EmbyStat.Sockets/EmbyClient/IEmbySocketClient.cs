using System.Threading;
using System.Threading.Tasks;

namespace EmbyStat.Sockets.EmbyClient
{
    public interface IEmbySocketClient
    {
        Task Connect(string url, CancellationToken cancellationToken);
        Task Close(CancellationToken cancellationToken);
        bool IsWebSocketConnectionOpen();
    }
}
