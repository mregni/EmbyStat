using EmbyStat.Clients.Base.WebSocket;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Clients.Jellyfin.WebSocket;

public class JellyfinWebSocketHandler : WebSocketHandler, IJellyfinWebSocketHandler
{
    public JellyfinWebSocketHandler(ILogger<WebSocketHandler> logger) : base(logger)
    {
    }
}