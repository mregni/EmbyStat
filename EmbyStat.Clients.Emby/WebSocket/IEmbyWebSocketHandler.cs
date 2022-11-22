﻿using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Base.WebSocket;
using WebSocketState = WebSocket4Net.WebSocketState;

namespace EmbyStat.Clients.Emby.WebSocket;

public interface IEmbyWebSocketHandler : IWebSocketHandler, IDisposable
{
    // event EventHandler Closed;
    // event EventHandler Connected;
    // Action<byte[]> OnReceiveBytes { get; set; }
    // Action<string> OnReceive { get; set; }
    // WebSocketState State { get; }
    // Task ConnectAsync(string url);
    // Task CloseConnection();
    // Task SendAsync(byte[] bytes, WebSocketMessageType type, bool endOfMessage, CancellationToken cancellationToken);
}