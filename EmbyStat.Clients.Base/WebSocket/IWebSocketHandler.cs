using System;
using System.Threading.Tasks;

namespace EmbyStat.Clients.Base.WebSocket;

public interface IWebSocketHandler
{
    #region Events

    event EventHandler OnWebSocketClosed;

    #endregion


    Task OpenWebSocket(string url, string accessToken, string deviceId);
    Task CloseWebSocket();
    Task StartReceivingSessionUpdates(int intervalMs);
    Task StopReceivingSessionUpdates();
}