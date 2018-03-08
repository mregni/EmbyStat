using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Connect;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Events;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.System;
using MediaBrowser.Model.Users;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Cryptography;
using Emby.ApiClient.Data;
using Emby.ApiClient.Model;
using Emby.ApiClient.Net;

namespace Emby.ApiClient
{
    public class ConnectionManager : IConnectionManager
    {
        public event EventHandler<GenericEventArgs<UserDto>> LocalUserSignIn;
        public event EventHandler<GenericEventArgs<ConnectUser>> ConnectUserSignIn;
        public event EventHandler<GenericEventArgs<IApiClient>> LocalUserSignOut;
        public event EventHandler<EventArgs> ConnectUserSignOut;
        public event EventHandler<EventArgs> RemoteLoggedOut;

        public event EventHandler<GenericEventArgs<ConnectionResult>> Connected;

        private readonly ICredentialProvider _credentialProvider;
        private readonly INetworkConnection _networkConnectivity;
        private readonly ILogger _logger;
        private readonly IServerLocator _serverDiscovery;
        private readonly IAsyncHttpClient _httpClient;
        private readonly Func<IClientWebSocket> _webSocketFactory;
        private readonly ICryptographyProvider _cryptographyProvider;
        private readonly ILocalAssetManager _localAssetManager;

        public Dictionary<string, IApiClient> ApiClients { get; private set; }

        public string ApplicationName { get; private set; }
        public string ApplicationVersion { get; private set; }
        public IDevice Device { get; private set; }
        public ClientCapabilities ClientCapabilities { get; private set; }

        public IApiClient CurrentApiClient { get; private set; }

        private readonly ConnectService _connectService;

        public ConnectUser ConnectUser { get; private set; }

        public ConnectionManager(ILogger logger,
            ICredentialProvider credentialProvider,
            INetworkConnection networkConnectivity,
            IServerLocator serverDiscovery,
            string applicationName,
            string applicationVersion,
            IDevice device,
            ClientCapabilities clientCapabilities,
            ICryptographyProvider cryptographyProvider,
            Func<IClientWebSocket> webSocketFactory = null,
            ILocalAssetManager localAssetManager = null)
        {
            _credentialProvider = credentialProvider;
            _networkConnectivity = networkConnectivity;
            _logger = logger;
            _serverDiscovery = serverDiscovery;
            _httpClient = AsyncHttpClientFactory.Create(logger);
            ClientCapabilities = clientCapabilities;
            _webSocketFactory = webSocketFactory;
            _cryptographyProvider = cryptographyProvider;
            _localAssetManager = localAssetManager;

            Device = device;
            ApplicationVersion = applicationVersion;
            ApplicationName = applicationName;
            ApiClients = new Dictionary<string, IApiClient>(StringComparer.OrdinalIgnoreCase);
            SaveLocalCredentials = true;

            var jsonSerializer = new NewtonsoftJsonSerializer();
            _connectService = new ConnectService(jsonSerializer, _logger, _httpClient, _cryptographyProvider, applicationName, applicationVersion);
        }

        public IJsonSerializer JsonSerializer
        {
            get { return _connectService.JsonSerializer; }
            set { _connectService.JsonSerializer = value; }
        }

        public bool SaveLocalCredentials { get; set; }

        private IApiClient GetOrAddApiClient(ServerInfo server, ConnectionMode connectionMode)
        {
            IApiClient apiClient;

            if (!ApiClients.TryGetValue(server.Id, out apiClient))
            {
                var address = server.GetAddress(connectionMode);

                apiClient = new ApiClient(_logger, address, ApplicationName, Device, ApplicationVersion, _cryptographyProvider, _localAssetManager)
                {
                    JsonSerializer = JsonSerializer,
                    OnAuthenticated = ApiClientOnAuthenticated
                };

                ApiClients[server.Id] = apiClient;
            }

            if (string.IsNullOrEmpty(server.AccessToken))
            {
                apiClient.ClearAuthenticationInfo();
            }
            else
            {
                apiClient.SetAuthenticationInfo(server.AccessToken, server.UserId);
            }

            return apiClient;
        }

        private Task ApiClientOnAuthenticated(IApiClient apiClient, AuthenticationResult result)
        {
            return OnAuthenticated(apiClient, result, new ConnectionOptions(), SaveLocalCredentials);
        }

        private async void AfterConnected(IApiClient apiClient, ConnectionOptions options)
        {
            if (options.ReportCapabilities)
            {
                try
                {
                    await apiClient.ReportCapabilities(ClientCapabilities).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Error reporting capabilities", ex);
                }
            }

            if (options.EnableWebSocket)
            {
                if (_webSocketFactory != null)
                {
                    ((ApiClient)apiClient).OpenWebSocket(_webSocketFactory);
                }
            }
        }

        public async Task<List<ServerInfo>> GetAvailableServers(CancellationToken cancellationToken = default(CancellationToken))
        {
            var credentials = await _credentialProvider.GetServerCredentials().ConfigureAwait(false);

            _logger.Debug("{0} servers in saved credentials", credentials.Servers.Count);

            if (_networkConnectivity.GetNetworkStatus().GetIsAnyLocalNetworkAvailable())
            {
                foreach (var server in await FindServers(cancellationToken).ConfigureAwait(false))
                {
                    credentials.AddOrUpdateServer(server);
                }
            }

            if (!string.IsNullOrWhiteSpace(credentials.ConnectAccessToken))
            {
                await EnsureConnectUser(credentials, cancellationToken).ConfigureAwait(false);

                var connectServers = await GetConnectServers(credentials.ConnectUserId, credentials.ConnectAccessToken, cancellationToken)
                            .ConfigureAwait(false);

                foreach (var server in connectServers)
                {
                    credentials.AddOrUpdateServer(server);
                }

                // Remove old servers
                var newServerList = credentials.Servers
                    .Where(i => string.IsNullOrWhiteSpace(i.ExchangeToken) ||
                        connectServers.Any(c => string.Equals(c.Id, i.Id, StringComparison.OrdinalIgnoreCase)));

                credentials.Servers = newServerList.ToList();
            }

            await _credentialProvider.SaveServerCredentials(credentials).ConfigureAwait(false);

            return credentials.Servers.OrderByDescending(i => i.DateLastAccessed).ToList();
        }

        private async Task<List<ServerInfo>> GetConnectServers(string userId, string accessToken, CancellationToken cancellationToken)
        {
            try
            {
                var servers = await _connectService.GetServers(userId, accessToken, cancellationToken).ConfigureAwait(false);

                _logger.Debug("User has {0} connect servers", servers.Length);

                return servers.Select(i => new ServerInfo
                {
                    ExchangeToken = i.AccessKey,
                    Id = i.SystemId,
                    Name = i.Name,
                    RemoteAddress = i.Url,
                    LocalAddress = i.LocalAddress,
                    UserLinkType = string.Equals(i.UserType, "guest", StringComparison.OrdinalIgnoreCase) ? UserLinkType.Guest : UserLinkType.LinkedUser

                }).ToList();
            }
            catch
            {
                return new List<ServerInfo>();
            }
        }

        private async Task<List<ServerInfo>> FindServers(CancellationToken cancellationToken)
        {
            List<ServerDiscoveryInfo> servers;

            try
            {
                servers = await _serverDiscovery.FindServers(1500, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("No servers found via local discovery.");

                servers = new List<ServerDiscoveryInfo>();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error discovering servers.", ex);

                servers = new List<ServerDiscoveryInfo>();
            }

            return servers.Select(i => new ServerInfo
            {
                Id = i.Id,
                LocalAddress = ConvertEndpointAddressToManualAddress(i) ?? i.Address,
                Name = i.Name
            })
            .ToList();
        }

        private string ConvertEndpointAddressToManualAddress(ServerDiscoveryInfo info)
        {
            if (!string.IsNullOrWhiteSpace(info.Address) && !string.IsNullOrWhiteSpace(info.EndpointAddress))
            {
                var address = info.EndpointAddress.Split(':').First();

                // Determine the port, if any
                var parts = info.Address.Split(':');
                if (parts.Length > 1)
                {
                    var portString = parts.Last();
                    int port;
                    if (int.TryParse(portString, NumberStyles.Any, CultureInfo.InvariantCulture, out port))
                    {
                        address += ":" + portString;
                    }
                }

                return NormalizeAddress(address);
            }

            return null;
        }

        public async Task<ConnectionResult> Connect(CancellationToken cancellationToken = default(CancellationToken))
        {
            var servers = await GetAvailableServers(cancellationToken).ConfigureAwait(false);

            return await Connect(servers, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Loops through a list of servers and returns the first that is available for connection
        /// </summary>
        private async Task<ConnectionResult> Connect(List<ServerInfo> servers, CancellationToken cancellationToken)
        {
            servers = servers
               .OrderByDescending(i => i.DateLastAccessed)
               .ToList();

            if (servers.Count == 1)
            {
                _logger.Debug("1 server in the list.");

                var result = await Connect(servers[0], cancellationToken).ConfigureAwait(false);

                if (result.State == ConnectionState.Unavailable)
                {
                    result.State = result.ConnectUser == null ?
                        ConnectionState.ConnectSignIn :
                        ConnectionState.ServerSelection;
                }

                return result;
            }

            var firstServer = servers.FirstOrDefault();
            // See if we have any saved credentials and can auto sign in
            if (firstServer != null && !string.IsNullOrEmpty(firstServer.AccessToken))
            {
                var result = await Connect(firstServer, cancellationToken).ConfigureAwait(false);

                if (result.State == ConnectionState.SignedIn)
                {
                    return result;
                }
            }

            var finalResult = new ConnectionResult
            {
                Servers = servers,
                ConnectUser = ConnectUser
            };

            finalResult.State = servers.Count == 0 && finalResult.ConnectUser == null ?
                ConnectionState.ConnectSignIn :
                ConnectionState.ServerSelection;

            return finalResult;
        }

        /// <summary>
        /// Attempts to connect to a server
        /// </summary>
        public Task<ConnectionResult> Connect(ServerInfo server, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Connect(server, new ConnectionOptions(), cancellationToken);
        }

        public async Task<ConnectionResult> Connect(ServerInfo server, ConnectionOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new ConnectionResult
            {
                State = ConnectionState.Unavailable
            };

            PublicSystemInfo systemInfo = null;
            var connectionMode = ConnectionMode.Manual;

            var tests = new[] { ConnectionMode.Manual, ConnectionMode.Local, ConnectionMode.Remote }.ToList();

            // If we've connected to the server before, try to optimize by starting with the last used connection mode
            if (server.LastConnectionMode.HasValue)
            {
                tests.Remove(server.LastConnectionMode.Value);
                tests.Insert(0, server.LastConnectionMode.Value);
            }

            var isLocalNetworkAvailable = _networkConnectivity.GetNetworkStatus().GetIsAnyLocalNetworkAvailable();

            // Kick off wake on lan on a separate thread (if applicable)
            var sendWakeOnLan = server.WakeOnLanInfos.Count > 0 && isLocalNetworkAvailable;

            var wakeOnLanTask = sendWakeOnLan ?
                Task.Run(() => WakeServer(server, cancellationToken), cancellationToken) :
                Task.FromResult(true);

            var wakeOnLanSendTime = DateTime.Now;

            foreach (var mode in tests)
            {
                _logger.Debug("Attempting to connect to server {0}. ConnectionMode: {1}", server.Name, mode.ToString());

                if (mode == ConnectionMode.Local)
                {
                    // Try connect locally if there's a local address,
                    // and we're either on localhost or the device has a local connection
                    if (!string.IsNullOrEmpty(server.LocalAddress) && isLocalNetworkAvailable)
                    {
                        // Try to connect to the local address
                        systemInfo = await TryConnect(server.LocalAddress, 8000, cancellationToken).ConfigureAwait(false);
                    }
                }
                else if (mode == ConnectionMode.Manual)
                {
                    // Try manual address if there is one, but only if it's different from the local/remote addresses
                    if (!string.IsNullOrEmpty(server.ManualAddress)
                        && !string.Equals(server.ManualAddress, server.LocalAddress, StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(server.ManualAddress, server.RemoteAddress, StringComparison.OrdinalIgnoreCase))
                    {
                        // Try to connect to the local address
                        systemInfo = await TryConnect(server.ManualAddress, 15000, cancellationToken).ConfigureAwait(false);
                    }
                }
                else if (mode == ConnectionMode.Remote)
                {
                    if (!string.IsNullOrEmpty(server.RemoteAddress))
                    {
                        systemInfo = await TryConnect(server.RemoteAddress, 15000, cancellationToken).ConfigureAwait(false);
                    }
                }

                if (systemInfo != null)
                {
                    connectionMode = mode;
                    break;
                }
            }

            if (systemInfo == null && !string.IsNullOrEmpty(server.LocalAddress) && isLocalNetworkAvailable && sendWakeOnLan)
            {
                await wakeOnLanTask.ConfigureAwait(false);

                // After wake on lan finishes, make sure at least 10 seconds have elapsed since the time it was first sent out
                var waitTime = TimeSpan.FromSeconds(10).TotalMilliseconds -
                               (DateTime.Now - wakeOnLanSendTime).TotalMilliseconds;

                if (waitTime > 0)
                {
                    await Task.Delay(Convert.ToInt32(waitTime, CultureInfo.InvariantCulture), cancellationToken).ConfigureAwait(false);
                }

                systemInfo = await TryConnect(server.LocalAddress, 15000, cancellationToken).ConfigureAwait(false);
            }

            if (systemInfo != null)
            {
                await OnSuccessfulConnection(server, options, systemInfo, result, connectionMode, cancellationToken)
                        .ConfigureAwait(false);
            }

            result.ConnectUser = ConnectUser;
            return result;
        }

        private async Task OnSuccessfulConnection(ServerInfo server,
            ConnectionOptions options,
            PublicSystemInfo systemInfo,
            ConnectionResult result,
            ConnectionMode connectionMode,
            CancellationToken cancellationToken)
        {
            server.ImportInfo(systemInfo);

            var credentials = await _credentialProvider.GetServerCredentials().ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(credentials.ConnectAccessToken))
            {
                await EnsureConnectUser(credentials, cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(server.ExchangeToken))
                {
                    await AddAuthenticationInfoFromConnect(server, connectionMode, credentials, cancellationToken).ConfigureAwait(false);
                }
            }

            if (!string.IsNullOrWhiteSpace(server.AccessToken))
            {
                await ValidateAuthentication(server, connectionMode, options, cancellationToken).ConfigureAwait(false);
            }

            credentials.AddOrUpdateServer(server);

            if (options.UpdateDateLastAccessed)
            {
                server.DateLastAccessed = DateTime.UtcNow;
            }
            server.LastConnectionMode = connectionMode;

            await _credentialProvider.SaveServerCredentials(credentials).ConfigureAwait(false);

            result.ApiClient = GetOrAddApiClient(server, connectionMode);
            result.State = string.IsNullOrEmpty(server.AccessToken) ?
                ConnectionState.ServerSignIn :
                ConnectionState.SignedIn;

            ((ApiClient)result.ApiClient).EnableAutomaticNetworking(server, connectionMode, _networkConnectivity);

            if (result.State == ConnectionState.SignedIn)
            {
                AfterConnected(result.ApiClient, options);
            }

            CurrentApiClient = result.ApiClient;

            result.Servers.Add(server);

            if (Connected != null)
            {
                Connected(this, new GenericEventArgs<ConnectionResult>(result));
            }
        }

        public Task<ConnectionResult> Connect(IApiClient apiClient, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = (ApiClient)apiClient;
            return Connect(client.ServerInfo, cancellationToken);
        }

        private async Task AddAuthenticationInfoFromConnect(ServerInfo server,
            ConnectionMode connectionMode,
            ServerCredentials credentials,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(credentials.ConnectUserId))
            {
                throw new ArgumentException("server");
            }

            if (string.IsNullOrWhiteSpace(server.ExchangeToken))
            {
                throw new ArgumentException("server");
            }

            _logger.Debug("Adding authentication info from Connect");

            var url = server.GetAddress(connectionMode);

            url += "/emby/Connect/Exchange?format=json&ConnectUserId=" + credentials.ConnectUserId;

            var headers = new HttpHeaders();
            headers.SetAccessToken(server.ExchangeToken);
            headers["X-Emby-Authorization"] = "MediaBrowser Client=\"" + ApplicationName + "\", Device=\"" + Device.DeviceName + "\", DeviceId=\"" + Device.DeviceId + "\", Version=\"" + ApplicationVersion + "\"";

            try
            {
                using (var stream = await _httpClient.SendAsync(new HttpRequest
                {
                    CancellationToken = cancellationToken,
                    Method = "GET",
                    RequestHeaders = headers,
                    Url = url

                }).ConfigureAwait(false))
                {
                    var auth = JsonSerializer.DeserializeFromStream<ConnectAuthenticationExchangeResult>(stream);

                    server.UserId = auth.LocalUserId;
                    server.AccessToken = auth.AccessToken;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Already logged at a lower level

                server.UserId = null;
                server.AccessToken = null;
            }
        }

        private async Task EnsureConnectUser(ServerCredentials credentials, CancellationToken cancellationToken)
        {
            if (ConnectUser != null && string.Equals(ConnectUser.Id, credentials.ConnectUserId, StringComparison.Ordinal))
            {
                return;
            }

            ConnectUser = null;

            if (!string.IsNullOrWhiteSpace(credentials.ConnectUserId) && !string.IsNullOrWhiteSpace(credentials.ConnectAccessToken))
            {
                try
                {
                    ConnectUser = await _connectService.GetConnectUser(new ConnectUserQuery
                    {
                        Id = credentials.ConnectUserId

                    }, credentials.ConnectAccessToken, cancellationToken).ConfigureAwait(false);

                    OnConnectUserSignIn(ConnectUser);
                }
                catch
                {
                    // Already logged at lower levels
                }
            }
        }

        private async Task ValidateAuthentication(ServerInfo server, ConnectionMode connectionMode, ConnectionOptions options, CancellationToken cancellationToken)
        {
            _logger.Debug("Validating saved authentication");

            var url = server.GetAddress(connectionMode);

            var headers = new HttpHeaders();
            headers.SetAccessToken(server.AccessToken);

            var request = new HttpRequest
            {
                CancellationToken = cancellationToken,
                Method = "GET",
                RequestHeaders = headers,
                Url = url + "/emby/system/info?format=json"
            };

            try
            {
                using (var stream = await _httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    var systemInfo = JsonSerializer.DeserializeFromStream<SystemInfo>(stream);

                    server.ImportInfo(systemInfo);
                }

                if (!string.IsNullOrEmpty(server.UserId))
                {
                    request.Url = url + "/mediabrowser/users/" + server.UserId + "?format=json";

                    using (var stream = await _httpClient.SendAsync(request).ConfigureAwait(false))
                    {
                        var localUser = JsonSerializer.DeserializeFromStream<UserDto>(stream);

                        OnLocalUserSignIn(options, localUser);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Already logged at a lower level

                server.UserId = null;
                server.AccessToken = null;
            }
        }

        private async Task<PublicSystemInfo> TryConnect(string url, int timeout, CancellationToken cancellationToken)
        {
            url += "/emby/system/info/public?format=json";

            try
            {
                using (var stream = await _httpClient.SendAsync(new HttpRequest
                {
                    Url = url,
                    CancellationToken = cancellationToken,
                    Timeout = timeout,
                    Method = "GET"

                }).ConfigureAwait(false))
                {
                    return JsonSerializer.DeserializeFromStream<PublicSystemInfo>(stream);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Already logged at a lower level

                return null;
            }
        }

        /// <summary>
        /// Wakes all servers.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        private async Task WakeAllServers(CancellationToken cancellationToken)
        {
            var credentials = await _credentialProvider.GetServerCredentials().ConfigureAwait(false);

            foreach (var server in credentials.Servers.ToList())
            {
                await WakeServer(server, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Wakes a server
        /// </summary>
        private async Task WakeServer(ServerInfo server, CancellationToken cancellationToken)
        {
            foreach (var info in server.WakeOnLanInfos)
            {
                await WakeServer(info, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Wakes a device based on mac address
        /// </summary>
        private async Task WakeServer(WakeOnLanInfo info, CancellationToken cancellationToken)
        {
            try
            {
                await _networkConnectivity.SendWakeOnLan(info.MacAddress, info.Port, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error sending wake on lan command", ex);
            }
        }

        public void Dispose()
        {
            foreach (var client in ApiClients.Values.ToList())
            {
                client.Dispose();
            }
        }

        public IApiClient GetApiClient(IHasServerId item)
        {
            return GetApiClient(item.ServerId);
        }

        public IApiClient GetApiClient(string serverId)
        {
            return ApiClients.Values.OfType<ApiClient>().FirstOrDefault(i => string.Equals(i.ServerInfo.Id, serverId, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ConnectionResult> Connect(string address, CancellationToken cancellationToken = default(CancellationToken))
        {
            address = NormalizeAddress(address);

            var publicInfo = await TryConnect(address, 15000, cancellationToken).ConfigureAwait(false);

            if (publicInfo == null)
            {
                return new ConnectionResult
                {
                    State = ConnectionState.Unavailable,
                    ConnectUser = ConnectUser
                };
            }

            var server = new ServerInfo
            {
                ManualAddress = address,
                LastConnectionMode = ConnectionMode.Manual
            };

            server.ImportInfo(publicInfo);

            return await Connect(server, cancellationToken).ConfigureAwait(false);
        }

        private string NormalizeAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("address");
            }

            if (!address.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                address = "http://" + address;
            }

            return address;
        }

        private async Task OnAuthenticated(IApiClient apiClient,
            AuthenticationResult result,
            ConnectionOptions options,
            bool saveCredentials)
        {
            var server = ((ApiClient)apiClient).ServerInfo;

            var credentials = await _credentialProvider.GetServerCredentials().ConfigureAwait(false);

            if (options.UpdateDateLastAccessed)
            {
                server.DateLastAccessed = DateTime.UtcNow;
            }

            if (saveCredentials)
            {
                server.UserId = result.User.Id;
                server.AccessToken = result.AccessToken;
            }
            else
            {
                server.UserId = null;
                server.AccessToken = null;
            }

            credentials.AddOrUpdateServer(server);
            await _credentialProvider.SaveServerCredentials(credentials).ConfigureAwait(false);

            AfterConnected(apiClient, options);

            OnLocalUserSignIn(options, result.User);
        }

        private void OnLocalUserSignIn(ConnectionOptions options, UserDto user)
        {
            // TODO: Create a separate property for this
            if (options.UpdateDateLastAccessed)
            {
                if (LocalUserSignIn != null)
                {
                    LocalUserSignIn(this, new GenericEventArgs<UserDto>(user));
                }
            }
        }

        private void OnLocalUserSignout(IApiClient apiClient)
        {
            if (LocalUserSignOut != null)
            {
                LocalUserSignOut(this, new GenericEventArgs<IApiClient>(apiClient));
            }
        }

        private void OnConnectUserSignIn(ConnectUser user)
        {
            ConnectUser = user;

            if (ConnectUserSignIn != null)
            {
                ConnectUserSignIn(this, new GenericEventArgs<ConnectUser>(ConnectUser));
            }
        }

        public async Task Logout()
        {
            foreach (var client in ApiClients.Values.ToList())
            {
                if (!string.IsNullOrEmpty(client.AccessToken))
                {
                    await client.Logout().ConfigureAwait(false);
                    OnLocalUserSignout(client);
                }
            }

            var credentials = await _credentialProvider.GetServerCredentials().ConfigureAwait(false);

            var servers = credentials.Servers
                .Where(i => !i.UserLinkType.HasValue || i.UserLinkType.Value != UserLinkType.Guest)
                .ToList();

            foreach (var server in servers)
            {
                server.AccessToken = null;
                server.UserId = null;
                server.ExchangeToken = null;
            }

            credentials.Servers = servers;
            credentials.ConnectAccessToken = null;
            credentials.ConnectUserId = null;

            await _credentialProvider.SaveServerCredentials(credentials).ConfigureAwait(false);

            if (ConnectUser != null)
            {
                ConnectUser = null;

                if (ConnectUserSignOut != null)
                {
                    ConnectUserSignOut(this, EventArgs.Empty);
                }
            }
        }

        public async Task LoginToConnect(string username, string password)
        {
            var result = await _connectService.Authenticate(username, password).ConfigureAwait(false);

            var credentials = await _credentialProvider.GetServerCredentials().ConfigureAwait(false);

            credentials.ConnectAccessToken = result.AccessToken;
            credentials.ConnectUserId = result.User.Id;

            await _credentialProvider.SaveServerCredentials(credentials).ConfigureAwait(false);

            OnConnectUserSignIn(result.User);
        }

        public Task<PinCreationResult> CreatePin()
        {
            return _connectService.CreatePin(Device.DeviceId);
        }

        public Task<PinStatusResult> GetPinStatus(PinCreationResult pin)
        {
            return _connectService.GetPinStatus(pin);
        }

        public async Task ExchangePin(PinCreationResult pin)
        {
            var result = await _connectService.ExchangePin(pin);

            var credentials = await _credentialProvider.GetServerCredentials().ConfigureAwait(false);

            credentials.ConnectAccessToken = result.AccessToken;
            credentials.ConnectUserId = result.UserId;

            await EnsureConnectUser(credentials, CancellationToken.None).ConfigureAwait(false);

            await _credentialProvider.SaveServerCredentials(credentials).ConfigureAwait(false);
        }

        public async Task<ServerInfo> GetServerInfo(string id)
        {
            var credentials = await _credentialProvider.GetServerCredentials().ConfigureAwait(false);

            return credentials.Servers.FirstOrDefault(i => string.Equals(i.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        public Task<ConnectSignupResponse> SignupForConnect(string email, string username, string password, CancellationToken cancellationToken)
        {
            return _connectService.SignupForConnect(email, username, password);
        }
    }
}
