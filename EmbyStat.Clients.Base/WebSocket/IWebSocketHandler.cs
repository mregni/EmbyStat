using System;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Sessions;

namespace EmbyStat.Clients.Base.WebSocket;

public interface IWebSocketHandler
{
    #region Events

    event EventHandler OnWebSocketClosed;
    event EventHandler<GenericEventArgs<WebSocketSession[]>> SessionsUpdated;

    #endregion


    Task OpenWebSocket(string url, string accessToken, string deviceId);
    Task CloseWebSocket();
    void StartReceivingSessionUpdates(int intervalMs);
    void StopReceivingSessionUpdates();
}