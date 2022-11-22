using System;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Clients.Base.WebSocket;

public interface IWebSocketApi
{
    event EventHandler OnWebSocketClosed;
    event EventHandler OnWebSocketConnected;
    event EventHandler<GenericEventArgs<JArray>> UserDataChanged;
    event EventHandler<GenericEventArgs<string>> UserDeleted;
    event EventHandler<GenericEventArgs<JObject>> UserUpdated;
    event EventHandler<EventArgs> ServerRestarting;
    event EventHandler<EventArgs> ServerShuttingDown;
    event EventHandler<GenericEventArgs<JArray>> SessionsUpdated;
    event EventHandler<EventArgs> RestartRequired;
}