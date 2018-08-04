using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmbyStat.Api.WebSocketClient
{
    public interface IWebSocketClient
    {
        Task Connect(string url, CancellationToken cancellationToken);
        Task Close(CancellationToken cancellationToken);
        bool IsWebSocketConnectionOpen();
    }
}
