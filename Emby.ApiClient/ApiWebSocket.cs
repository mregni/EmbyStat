using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Events;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Sync;
using MediaBrowser.Model.Tasks;
using MediaBrowser.Model.Updates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Model;
using Newtonsoft.Json;

namespace Emby.ApiClient
{
    public partial class ApiClient : IServerEvents, IDisposable
    {
        /// <summary>
        /// Occurs when [closed].
        /// </summary>
        public event EventHandler WebSocketClosed;
        public event EventHandler<GenericEventArgs<GeneralCommandEventArgs>> GeneralCommand;
        public event EventHandler<GenericEventArgs<BrowseRequest>> BrowseCommand;
        public event EventHandler<GenericEventArgs<LibraryUpdateInfo>> LibraryChanged;
        public event EventHandler<GenericEventArgs<MessageCommand>> MessageCommand;
        public event EventHandler<GenericEventArgs<InstallationInfo>> PackageInstallationCancelled;
        public event EventHandler<GenericEventArgs<InstallationInfo>> PackageInstallationCompleted;
        public event EventHandler<GenericEventArgs<InstallationInfo>> PackageInstallationFailed;
        public event EventHandler<GenericEventArgs<InstallationInfo>> PackageInstalling;
        public event EventHandler<GenericEventArgs<PlayRequest>> PlayCommand;
        public event EventHandler<GenericEventArgs<PlaystateRequest>> PlaystateCommand;
        public event EventHandler<GenericEventArgs<PluginInfo>> PluginUninstalled;
        public event EventHandler<GenericEventArgs<TaskResult>> ScheduledTaskEnded;
        public event EventHandler<GenericEventArgs<string>> SendStringCommand;
        public event EventHandler<GenericEventArgs<int>> SetAudioStreamIndexCommand;
        public event EventHandler<GenericEventArgs<int>> SetSubtitleStreamIndexCommand;
        public event EventHandler<GenericEventArgs<int>> SetVolumeCommand;
        public event EventHandler<GenericEventArgs<UserDataChangeInfo>> UserDataChanged;
        public event EventHandler<GenericEventArgs<string>> UserDeleted;
        public event EventHandler<GenericEventArgs<UserDto>> UserUpdated;
        public event EventHandler<EventArgs> NotificationAdded;
        public event EventHandler<EventArgs> NotificationUpdated;
        public event EventHandler<EventArgs> NotificationsMarkedRead;
        public event EventHandler<EventArgs> ServerRestarting;
        public event EventHandler<EventArgs> ServerShuttingDown;
        public event EventHandler<EventArgs> WebSocketConnected;
        public event EventHandler<GenericEventArgs<SessionUpdatesEventArgs>> SessionsUpdated;
        public event EventHandler<EventArgs> RestartRequired;
        public event EventHandler<GenericEventArgs<SessionInfoDto>> PlaybackStart;
        public event EventHandler<GenericEventArgs<SessionInfoDto>> PlaybackStopped;
        public event EventHandler<GenericEventArgs<SessionInfoDto>> SessionEnded;
        public event EventHandler<GenericEventArgs<SyncJobCreationResult>> SyncJobCreated;
        public event EventHandler<GenericEventArgs<SyncJob>> SyncJobCancelled;
        public event EventHandler<GenericEventArgs<List<SyncJob>>> SyncJobsUpdated;
        public event EventHandler<GenericEventArgs<CompleteSyncJobInfo>> SyncJobUpdated;

        /// <summary>
        /// The _web socket
        /// </summary>
        private Func<IClientWebSocket> _webSocketFactory;

        /// <summary>
        /// The _current web socket
        /// </summary>
        private IClientWebSocket _currentWebSocket;

        /// <summary>
        /// Creates the specified logger.
        /// </summary>
        public void OpenWebSocket(Func<IClientWebSocket> webSocketFactory)
        {
            _webSocketFactory = webSocketFactory;

            if (!IsWebSocketOpenOrConnecting)
            {
                CloseWebSocket();
                Task.Factory.StartNew(() => EnsureConnectionAsync(CancellationToken.None));

            }
        }

        public void CloseWebSocket()
        {
            DisposeWebSocket();
        }

        /// <summary>
        /// Ensures the connection.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        private async Task EnsureConnectionAsync(CancellationToken cancellationToken)
        {
            if (!IsWebSocketOpenOrConnecting)
            {
                var url = GetWebSocketUrl(ApiUrl);

                try
                {
                    var socket = _webSocketFactory();

                    Logger.Info("Created new web socket of type {0}", socket.GetType().Name);

                    Logger.Info("Connecting to {0}", url);

                    await socket.ConnectAsync(url, cancellationToken).ConfigureAwait(false);

                    Logger.Info("Connected to {0}", url);

                    socket.OnReceiveBytes = OnMessageReceived;
                    socket.OnReceive = OnMessageReceived;
                    socket.Closed += _currentWebSocket_Closed;

                    //try
                    //{
                    //    await SendWebSocketMessage("Identity", GetIdentificationMessage()).ConfigureAwait(false);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Logger.ErrorException("Error sending identity message", ex);
                    //}

                    ReplaceSocket(socket);

                    OnConnected();
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Error connecting to {0}", ex, url);
                }
            }
        }

        /// <summary>
        /// Replaces the socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        private void ReplaceSocket(IClientWebSocket socket)
        {
            var previousSocket = _currentWebSocket;

            _currentWebSocket = socket;

            if (previousSocket != null)
            {
                previousSocket.Dispose();
            }
        }

        /// <summary>
        /// Handles the WebSocketClosed event of the _currentWebSocket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void _currentWebSocket_Closed(object sender, EventArgs e)
        {
            Logger.Warn("Web socket connection closed.");

            if (WebSocketClosed != null)
            {
                WebSocketClosed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sends the async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageName">Name of the message.</param>
        /// <param name="data">The data.</param>
        /// <returns>Task.</returns>
        public Task SendWebSocketMessage<T>(string messageName, T data)
        {
            return SendWebSocketMessage(messageName, data, CancellationToken.None);
        }

        /// <summary>
        /// Sends the async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageName">Name of the message.</param>
        /// <param name="data">The data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task SendWebSocketMessage<T>(string messageName, T data, CancellationToken cancellationToken)
        {
            var bytes = GetMessageBytes(messageName, data);

            try
            {
                await _currentWebSocket.SendAsync(bytes, WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Error sending web socket message", ex);

                throw;
            }
        }

        /// <summary>
        /// Sends the server a message indicating what is currently being viewed by the client
        /// </summary>
        /// <param name="itemType">The current item type (if any)</param>
        /// <param name="itemId">The current item id (if any)</param>
        /// <param name="itemName">The current item name (if any)</param>
        /// <param name="context">An optional, client-specific value indicating the area or section being browsed</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public Task SendContextMessageAsync(string itemType, string itemId, string itemName, string context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var vals = new List<string>
                {
                    itemType ?? string.Empty, 
                    itemId ?? string.Empty, 
                    itemName ?? string.Empty
                };

            if (!string.IsNullOrEmpty(context))
            {
                vals.Add(context);
            }

            return SendWebSocketMessage("Context", string.Join("|", vals.ToArray()), cancellationToken);
        }

        /// <summary>
        /// Starts the receiving session updates.
        /// </summary>
        /// <param name="intervalMs">The interval ms.</param>
        /// <returns>Task.</returns>
        public Task StartReceivingSessionUpdates(int intervalMs)
        {
            return SendWebSocketMessage("SessionsStart", string.Format("{0},{0}", intervalMs));
        }

        /// <summary>
        /// Stops the receiving session updates.
        /// </summary>
        /// <returns>Task.</returns>
        public Task StopReceivingSessionUpdates()
        {
            return SendWebSocketMessage("SessionsStop", string.Empty);
        }

        public Task StartReceivingSyncJobsUpdates(int intervalMs, string userId, string targetId)
        {
            var options = new List<string>();
            options.Add(intervalMs.ToString(CultureInfo.InvariantCulture));
            options.Add(intervalMs.ToString(CultureInfo.InvariantCulture));
            options.Add(userId ?? string.Empty);
            options.Add(targetId ?? string.Empty);

            return SendWebSocketMessage("SyncJobsStart", string.Join(",", options.ToArray()));
        }

        public Task StopReceivingSyncJobsUpdates()
        {
            return SendWebSocketMessage("SyncJobsStop", string.Empty);
        }

        public Task StartReceivingSyncJobUpdates(int intervalMs, string jobId)
        {
            var options = new List<string>();
            options.Add(intervalMs.ToString(CultureInfo.InvariantCulture));
            options.Add(intervalMs.ToString(CultureInfo.InvariantCulture));
            options.Add(jobId ?? string.Empty);

            return SendWebSocketMessage("SyncJobStart", string.Join(",", options.ToArray()));
        }

        public Task StopReceivingSyncJobUpdates()
        {
            return SendWebSocketMessage("SyncJobStop", string.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            DisposeWebSocket();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        private void DisposeWebSocket()
        {
            var socket = _currentWebSocket;

            if (socket != null)
            {
                Logger.Debug("Disposing client web socket");

                try
                {
                    socket.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Error disposing web socket {0}", ex);
                }
                _currentWebSocket = null;
            }
        }

        /// <summary>
        /// Gets the web socket URL.
        /// </summary>
        /// <param name="serverAddress">The server address.</param>
        /// <returns>System.String.</returns>
        protected string GetWebSocketUrl(string serverAddress)
        {
            if (string.IsNullOrWhiteSpace(AccessToken))
            {
                throw new ArgumentException("Cannot open web socket without an access token.");
            }

            return serverAddress.Replace("http:", "ws:").Replace("https:", "wss:") + "/embywebsocket?api_key=" + AccessToken + "&deviceId=" + DeviceId;
        }

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        protected void OnMessageReceived(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            OnMessageReceived(json);
        }

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="json">The json.</param>
        protected void OnMessageReceived(string json)
        {
            try
            {
                OnMessageReceivedInternal(json);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Error in OnMessageReceivedInternal", ex);
            }
        }

        private void OnMessageReceivedInternal(string json)
        {
            // deserialize the WebSocketMessage with an object payload
            var messageType = GetMessageType(json);

            Logger.Info("Received web socket message: {0}", messageType);

            if (string.Equals(messageType, "LibraryChanged"))
            {
                FireEvent(LibraryChanged, this, new GenericEventArgs<LibraryUpdateInfo>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<LibraryUpdateInfo>>(json).Data
                });
            }
            else if (string.Equals(messageType, "RestartRequired"))
            {
                FireEvent(RestartRequired, this, EventArgs.Empty);
            }
            else if (string.Equals(messageType, "ServerRestarting"))
            {
                FireEvent(ServerRestarting, this, EventArgs.Empty);
            }
            else if (string.Equals(messageType, "ServerShuttingDown"))
            {
                FireEvent(ServerShuttingDown, this, EventArgs.Empty);
            }
            else if (string.Equals(messageType, "UserDeleted"))
            {
                var userId = JsonSerializer.DeserializeFromString<WebSocketMessage<string>>(json).Data;

                FireEvent(UserDeleted, this, new GenericEventArgs<string>
                {
                    Argument = userId
                });
            }
            else if (string.Equals(messageType, "ScheduledTaskEnded"))
            {
                FireEvent(ScheduledTaskEnded, this, new GenericEventArgs<TaskResult>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<TaskResult>>(json).Data
                });
            }
            else if (string.Equals(messageType, "PackageInstalling"))
            {
                FireEvent(PackageInstalling, this, new GenericEventArgs<InstallationInfo>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<InstallationInfo>>(json).Data
                });
            }
            else if (string.Equals(messageType, "PackageInstallationFailed"))
            {
                FireEvent(PackageInstallationFailed, this, new GenericEventArgs<InstallationInfo>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<InstallationInfo>>(json).Data
                });
            }
            else if (string.Equals(messageType, "PackageInstallationCompleted"))
            {
                FireEvent(PackageInstallationCompleted, this, new GenericEventArgs<InstallationInfo>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<InstallationInfo>>(json).Data
                });
            }
            else if (string.Equals(messageType, "PackageInstallationCancelled"))
            {
                FireEvent(PackageInstallationCancelled, this, new GenericEventArgs<InstallationInfo>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<InstallationInfo>>(json).Data
                });
            }
            else if (string.Equals(messageType, "UserUpdated"))
            {
                FireEvent(UserUpdated, this, new GenericEventArgs<UserDto>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<UserDto>>(json).Data
                });
            }
            else if (string.Equals(messageType, "PluginUninstalled"))
            {
                FireEvent(PluginUninstalled, this, new GenericEventArgs<PluginInfo>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<PluginInfo>>(json).Data
                });
            }
            else if (string.Equals(messageType, "Play"))
            {
                FireEvent(PlayCommand, this, new GenericEventArgs<PlayRequest>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<PlayRequest>>(json).Data
                });
            }
            else if (string.Equals(messageType, "Playstate"))
            {
                FireEvent(PlaystateCommand, this, new GenericEventArgs<PlaystateRequest>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<PlaystateRequest>>(json).Data
                });
            }
            else if (string.Equals(messageType, "NotificationAdded"))
            {
                FireEvent(NotificationAdded, this, EventArgs.Empty);
            }
            else if (string.Equals(messageType, "NotificationUpdated"))
            {
                FireEvent(NotificationUpdated, this, EventArgs.Empty);
            }
            else if (string.Equals(messageType, "NotificationsMarkedRead"))
            {
                FireEvent(NotificationsMarkedRead, this, EventArgs.Empty);
            }
            else if (string.Equals(messageType, "GeneralCommand"))
            {
                OnGeneralCommand(json);
            }
            else if (string.Equals(messageType, "Sessions"))
            {
                FireEvent(SessionsUpdated, this, new GenericEventArgs<SessionUpdatesEventArgs>(new SessionUpdatesEventArgs
                {
                    Sessions = JsonSerializer.DeserializeFromString<WebSocketMessage<SessionInfoDto[]>>(json).Data
                }));
            }
            else if (string.Equals(messageType, "SyncJobs"))
            {
                FireEvent(SyncJobsUpdated, this, new GenericEventArgs<List<SyncJob>>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<List<SyncJob>>>(json).Data
                });
            }
            else if (string.Equals(messageType, "SyncJob"))
            {
                FireEvent(SyncJobUpdated, this, new GenericEventArgs<CompleteSyncJobInfo>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<CompleteSyncJobInfo>>(json).Data
                });
            }
            else if (string.Equals(messageType, "UserDataChanged"))
            {
                FireEvent(UserDataChanged, this, new GenericEventArgs<UserDataChangeInfo>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<UserDataChangeInfo>>(json).Data
                });
            }
            else if (string.Equals(messageType, "SessionEnded"))
            {
                FireEvent(SessionEnded, this, new GenericEventArgs<SessionInfoDto>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<SessionInfoDto>>(json).Data
                });
            }
            else if (string.Equals(messageType, "SyncJobCreated"))
            {
                FireEvent(SyncJobCreated, this, new GenericEventArgs<SyncJobCreationResult>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<SyncJobCreationResult>>(json).Data
                });
            }
            else if (string.Equals(messageType, "SyncJobCancelled"))
            {
                FireEvent(SyncJobCancelled, this, new GenericEventArgs<SyncJob>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<SyncJob>>(json).Data
                });
            }
            else if (string.Equals(messageType, "PlaybackStart"))
            {
                FireEvent(PlaybackStart, this, new GenericEventArgs<SessionInfoDto>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<SessionInfoDto>>(json).Data
                });
            }
            else if (string.Equals(messageType, "PlaybackStopped"))
            {
                FireEvent(PlaybackStopped, this, new GenericEventArgs<SessionInfoDto>
                {
                    Argument = JsonSerializer.DeserializeFromString<WebSocketMessage<SessionInfoDto>>(json).Data
                });
            }
        }

        private void OnGeneralCommand(string json)
        {
            var args = new GeneralCommandEventArgs
            {
                Command = JsonSerializer.DeserializeFromString<WebSocketMessage<GeneralCommand>>(json).Data
            };

            try
            {
                args.KnownCommandType = (GeneralCommandType)Enum.Parse(typeof(GeneralCommandType), args.Command.Name, true);
            }
            catch
            {
                // Could be a custom name.
            }

            if (args.KnownCommandType.HasValue)
            {
                if (args.KnownCommandType.Value == GeneralCommandType.DisplayContent)
                {
                    string itemId;
                    string itemName;
                    string itemType;

                    args.Command.Arguments.TryGetValue("ItemId", out itemId);
                    args.Command.Arguments.TryGetValue("ItemName", out itemName);
                    args.Command.Arguments.TryGetValue("ItemType", out itemType);

                    FireEvent(BrowseCommand, this, new GenericEventArgs<BrowseRequest>
                    {
                        Argument = new BrowseRequest
                        {
                            ItemId = itemId,
                            ItemName = itemName,
                            ItemType = itemType
                        }
                    });
                    return;
                }
                if (args.KnownCommandType.Value == GeneralCommandType.DisplayMessage)
                {
                    string header;
                    string text;
                    string timeoutMs;

                    args.Command.Arguments.TryGetValue("Header", out header);
                    args.Command.Arguments.TryGetValue("Text", out text);
                    args.Command.Arguments.TryGetValue("TimeoutMs", out timeoutMs);

                    long? timeoutVal = string.IsNullOrEmpty(timeoutMs) ? (long?)null : long.Parse(timeoutMs, CultureInfo.InvariantCulture);

                    FireEvent(MessageCommand, this, new GenericEventArgs<MessageCommand>
                    {
                        Argument = new MessageCommand
                        {
                            Header = header,
                            Text = text,
                            TimeoutMs = timeoutVal
                        }
                    });
                    return;
                }
                if (args.KnownCommandType.Value == GeneralCommandType.SetVolume)
                {
                    string volume;

                    args.Command.Arguments.TryGetValue("Volume", out volume);

                    FireEvent(SetVolumeCommand, this, new GenericEventArgs<int>
                    {
                        Argument = int.Parse(volume, CultureInfo.InvariantCulture)
                    });
                    return;
                }
                if (args.KnownCommandType.Value == GeneralCommandType.SetAudioStreamIndex)
                {
                    string index;

                    args.Command.Arguments.TryGetValue("Index", out index);

                    FireEvent(SetAudioStreamIndexCommand, this, new GenericEventArgs<int>
                    {
                        Argument = int.Parse(index, CultureInfo.InvariantCulture)
                    });
                    return;
                }
                if (args.KnownCommandType.Value == GeneralCommandType.SetSubtitleStreamIndex)
                {
                    string index;

                    args.Command.Arguments.TryGetValue("Index", out index);

                    FireEvent(SetSubtitleStreamIndexCommand, this, new GenericEventArgs<int>
                    {
                        Argument = int.Parse(index, CultureInfo.InvariantCulture)
                    });
                    return;
                }
                if (args.KnownCommandType.Value == GeneralCommandType.SendString)
                {
                    string val;

                    args.Command.Arguments.TryGetValue("String", out val);

                    FireEvent(SendStringCommand, this, new GenericEventArgs<string>
                    {
                        Argument = val
                    });
                    return;
                }
            }

            FireEvent(GeneralCommand, this, new GenericEventArgs<GeneralCommandEventArgs>(args));
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsWebSocketConnected
        {
            get { return _currentWebSocket != null && _currentWebSocket.State == WebSocketState.Open; }
        }

        private bool IsWebSocketOpenOrConnecting
        {
            get
            {
                return _currentWebSocket != null &&
                    (_currentWebSocket.State == WebSocketState.Open || _currentWebSocket.State == WebSocketState.Connecting);
            }
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>System.String.</returns>
        private string GetMessageType(string json)
        {
            var message = JsonSerializer.DeserializeFromString<WebSocketMessage>(json);
            return message.MessageType;
        }

        /// <summary>
        /// Called when [connected].
        /// </summary>
        protected void OnConnected()
        {
            FireEvent(WebSocketConnected, this, EventArgs.Empty);
        }

        /// <summary>
        /// Queues the event if not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        protected void FireEvent<T>(EventHandler<T> handler, object sender, T args)
            where T : EventArgs
        {
            if (handler != null)
            {
                try
                {
                    handler(sender, args);
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Error in event handler", ex);
                }
            }
        }

        /// <summary>
        /// Gets the message bytes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageName">Name of the message.</param>
        /// <param name="data">The data.</param>
        /// <returns>System.Byte[][].</returns>
        protected byte[] GetMessageBytes<T>(string messageName, T data)
        {
            var msg = new WebSocketMessage<T> { MessageType = messageName, Data = data };

            return SerializeToBytes(JsonSerializer, msg);
        }

        /// <summary>
        /// Serializes to bytes.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>System.Byte[][].</returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        private static byte[] SerializeToBytes(IJsonSerializer json, object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (var stream = new MemoryStream())
            {
                json.SerializeToStream(obj, stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Gets the identification message.
        /// </summary>
        /// <returns>System.String.</returns>
        protected string GetIdentificationMessage()
        {
            return ClientName + "|" + DeviceId + "|" + ApplicationVersion + "|" + DeviceName;
        }

        /// <summary>
        /// Class WebSocketMessage
        /// </summary>
        class WebSocketMessage
        {
            /// <summary>
            /// Gets or sets the type of the message.
            /// </summary>
            /// <value>The type of the message.</value>
            public string MessageType { get; set; }
        }
    }
}
