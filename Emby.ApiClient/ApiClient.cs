using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Devices;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Events;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Notifications;
using MediaBrowser.Model.Playlists;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Search;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Sync;
using MediaBrowser.Model.System;
using MediaBrowser.Model.Tasks;
using MediaBrowser.Model.Users;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Cryptography;
using Emby.ApiClient.Data;
using Emby.ApiClient.Model;
using Emby.ApiClient.Net;

namespace Emby.ApiClient
{
    /// <summary>
    /// Provides api methods centered around an HttpClient
    /// </summary>
    public partial class ApiClient : BaseApiClient, IApiClient
    {
        public event EventHandler<GenericEventArgs<RemoteLogoutReason>> RemoteLoggedOut;
        public event EventHandler<GenericEventArgs<AuthenticationResult>> Authenticated;

        /// <summary>
        /// Gets the HTTP client.
        /// </summary>
        /// <value>The HTTP client.</value>
        protected IAsyncHttpClient HttpClient { get; private set; }

        private readonly ICryptographyProvider _cryptographyProvider;
        private readonly ILocalAssetManager _localAssetManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serverAddress">The server address.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="cryptographyProvider">The cryptography provider.</param>
        public ApiClient(ILogger logger,
            string serverAddress,
            string accessToken,
            ICryptographyProvider cryptographyProvider)
            : base(logger, new NewtonsoftJsonSerializer(), serverAddress, accessToken)
        {
            CreateHttpClient(logger);
            _cryptographyProvider = cryptographyProvider;

            ResetHttpHeaders();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serverAddress">The server address.</param>
        /// <param name="clientName">Name of the client.</param>
        /// <param name="device">The device.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="cryptographyProvider">The cryptography provider.</param>
        public ApiClient(ILogger logger,
            string serverAddress,
            string clientName,
            IDevice device,
            string applicationVersion,
            ICryptographyProvider cryptographyProvider)
            : base(logger, new NewtonsoftJsonSerializer(), serverAddress, clientName, device, applicationVersion)
        {
            CreateHttpClient(logger);
            _cryptographyProvider = cryptographyProvider;

            ResetHttpHeaders();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serverAddress">The server address.</param>
        /// <param name="clientName">Name of the client.</param>
        /// <param name="device">The device.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="cryptographyProvider">The cryptography provider.</param>
        /// <param name="localAssetManager">The local asset manager.</param>
        public ApiClient(ILogger logger,
            string serverAddress,
            string clientName,
            IDevice device,
            string applicationVersion,
            ICryptographyProvider cryptographyProvider,
            ILocalAssetManager localAssetManager)
            : this(logger, serverAddress, clientName, device, applicationVersion, cryptographyProvider)
        {
            _localAssetManager = localAssetManager;
        }

        private void CreateHttpClient(ILogger logger)
        {
            HttpClient = AsyncHttpClientFactory.Create(logger);
            HttpClient.HttpResponseReceived += HttpClient_HttpResponseReceived;
        }

        void HttpClient_HttpResponseReceived(object sender, HttpResponseEventArgs e)
        {
            if (e.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (RemoteLoggedOut != null)
                {
                    RemoteLoggedOut(this, new GenericEventArgs<RemoteLogoutReason>());
                }
            }
        }

        private ConnectionMode ConnectionMode { get; set; }
        internal ServerInfo ServerInfo { get; set; }
        private INetworkConnection NetworkConnection { get; set; }

        public void EnableAutomaticNetworking(ServerInfo info, ConnectionMode initialMode, INetworkConnection networkConnection)
        {
            NetworkConnection = networkConnection;
            ConnectionMode = initialMode;
            ServerInfo = info;
            ServerAddress = info.GetAddress(initialMode);
        }

        private async Task<Stream> SendAsync(HttpRequest request, bool enableFailover = true)
        {
            // If not using automatic connection, execute the request directly
            if (NetworkConnection == null || !enableFailover)
            {
                return await HttpClient.SendAsync(request).ConfigureAwait(false);
            }

            var initialConnectionMode = ConnectionMode;
            var originalRequestTime = DateTime.UtcNow;
            Exception timeoutException;

            try
            {
                return await HttpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (HttpException ex)
            {
                if (!ex.IsTimedOut)
                {
                    throw;
                }

                timeoutException = ex;
            }

            try
            {
                await ValidateConnection(originalRequestTime, initialConnectionMode, request.CancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Unable to re-establish connection with the server. 
                // Throw the original exception
                throw timeoutException;
            }

            request.Url = ReplaceServerAddress(request.Url, initialConnectionMode);

            return await HttpClient.SendAsync(request).ConfigureAwait(false);
        }

        private readonly SemaphoreSlim _validateConnectionSemaphore = new SemaphoreSlim(1, 1);

        private DateTime _lastConnectionValidationTime = DateTime.MinValue;

        private async Task ValidateConnection(DateTime originalRequestTime, ConnectionMode initialConnectionMode, CancellationToken cancellationToken)
        {
            await _validateConnectionSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (originalRequestTime > _lastConnectionValidationTime)
                {
                    await ValidateConnectionInternal(initialConnectionMode, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _validateConnectionSemaphore.Release();
            }
        }

        private async Task ValidateConnectionInternal(ConnectionMode initialConnectionMode, CancellationToken cancellationToken)
        {
            Logger.Debug("Connection to server dropped. Attempting to reconnect.");

            const int maxWaitMs = 10000;
            const int waitIntervalMs = 100;
            var totalWaitMs = 0;
            var networkStatus = NetworkConnection.GetNetworkStatus();

            while (!networkStatus.IsNetworkAvailable)
            {
                if (totalWaitMs >= maxWaitMs)
                {
                    throw new Exception("Network unavailable.");
                }

                await Task.Delay(waitIntervalMs, cancellationToken).ConfigureAwait(false);

                totalWaitMs += waitIntervalMs;
                networkStatus = NetworkConnection.GetNetworkStatus();
            }

            var urlList = new List<Tuple<string, ConnectionMode>>
			{
				new Tuple<string,ConnectionMode>(ServerInfo.LocalAddress, ConnectionMode.Local),
				new Tuple<string,ConnectionMode>(ServerInfo.RemoteAddress, ConnectionMode.Remote)
			};

            if (!networkStatus.GetIsAnyLocalNetworkAvailable())
            {
                urlList.Reverse();
            }

            if (!string.IsNullOrEmpty(ServerInfo.ManualAddress))
            {
                if (!string.Equals(ServerInfo.ManualAddress, ServerInfo.LocalAddress, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(ServerInfo.ManualAddress, ServerInfo.RemoteAddress, StringComparison.OrdinalIgnoreCase))
                {
                    urlList.Insert(0, new Tuple<string, ConnectionMode>(ServerInfo.ManualAddress, ConnectionMode.Manual));
                }
            }

            foreach (var url in urlList)
            {
                var connected = await TryConnect(url.Item1, cancellationToken).ConfigureAwait(false);

                if (connected)
                {
                    ConnectionMode = url.Item2;
                    break;
                }
            }

            _lastConnectionValidationTime = DateTime.UtcNow;
        }

        private async Task<bool> TryConnect(string baseUrl, CancellationToken cancellationToken)
        {
            var fullUrl = baseUrl + "/system/info/public";

            fullUrl = AddDataFormat(fullUrl);

            var request = new HttpRequest
            {
                Url = fullUrl,
                RequestHeaders = HttpHeaders,
                CancellationToken = cancellationToken,
                Method = "GET"
            };

            try
            {
                using (var stream = await HttpClient.SendAsync(request).ConfigureAwait(false))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string ReplaceServerAddress(string url, ConnectionMode initialConnectionMode)
        {
            var baseUrl = ServerInfo.GetAddress(ConnectionMode);

            var index = url.IndexOf("/mediabrowser", StringComparison.OrdinalIgnoreCase);

            if (index != -1)
            {
                return baseUrl.TrimEnd('/') + url.Substring(index);
            }

            return url;

            //return url.Replace(ServerInfo.GetAddress(initialConnectionMode), ServerInfo.GetAddress(ConnectionMode), StringComparison.OrdinalIgnoreCase);
        }

        public Task<Stream> GetStream(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync(new HttpRequest
            {
                CancellationToken = cancellationToken,
                Method = "GET",
                RequestHeaders = HttpHeaders,
                Url = url
            });
        }


        public Task<HttpResponse> GetResponse(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            return HttpClient.GetResponse(new HttpRequest
            {
                CancellationToken = cancellationToken,
                Method = "GET",
                RequestHeaders = HttpHeaders,
                Url = url
            });
        }

        /// <summary>
        /// Gets an image stream based on a url
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{Stream}.</returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        public Task<Stream> GetImageStreamAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            return GetStream(url, cancellationToken);
        }

        /// <summary>
        /// Gets a BaseItem
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Task{BaseItemDto}.</returns>
        /// <exception cref="System.ArgumentNullException">id</exception>
        public async Task<BaseItemDto> GetItemAsync(string id, string userId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var url = GetApiUrl("Users/" + userId + "/Items/" + id);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<BaseItemDto>(stream);
            }
        }

        /// <summary>
        /// Gets the intros async.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Task{System.String[]}.</returns>
        /// <exception cref="System.ArgumentNullException">id</exception>
        public async Task<QueryResult<BaseItemDto>> GetIntrosAsync(string itemId, string userId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentNullException("itemId");
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var url = GetApiUrl("Users/" + userId + "/Items/" + itemId + "/Intros");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        /// <summary>
        /// Gets the item counts async.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Task{ItemCounts}.</returns>
        /// <exception cref="System.ArgumentNullException">query</exception>
        public async Task<ItemCounts> GetItemCountsAsync(ItemCountsQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("UserId", query.UserId);
            dict.AddIfNotNull("IsFavorite", query.IsFavorite);

            var url = GetApiUrl("Items/Counts", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<ItemCounts>(stream);
            }
        }

        /// <summary>
        /// Gets a BaseItem
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Task{BaseItemDto}.</returns>
        /// <exception cref="System.ArgumentNullException">userId</exception>
        public async Task<BaseItemDto> GetRootFolderAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var url = GetApiUrl("Users/" + userId + "/Items/Root");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<BaseItemDto>(stream);
            }
        }

        /// <summary>
        /// Gets the users async.
        /// </summary>
        /// <returns>Task{UserDto[]}.</returns>
        public async Task<UserDto[]> GetUsersAsync(UserQuery query)
        {
            var queryString = new QueryStringDictionary();

            queryString.AddIfNotNull("IsDisabled", query.IsDisabled);
            queryString.AddIfNotNull("IsHidden", query.IsHidden);

            var url = GetApiUrl("Users", queryString);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<UserDto[]>(stream);
            }
        }

        public async Task<UserDto[]> GetPublicUsersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = GetApiUrl("Users/Public");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<UserDto[]>(stream);
            }
        }

        /// <summary>
        /// Gets active client sessions.
        /// </summary>
        /// <returns>Task{SessionInfoDto[]}.</returns>
        public async Task<SessionInfoDto[]> GetClientSessionsAsync(SessionQuery query)
        {
            var queryString = new QueryStringDictionary();

            queryString.AddIfNotNullOrEmpty("ControllableByUserId", query.ControllableByUserId);

            var url = GetApiUrl("Sessions", queryString);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<SessionInfoDto[]>(stream);
            }
        }

        public Task<PluginSecurityInfo> GetRegistrationInfo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries for items
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Task{ItemsResult}.</returns>
        /// <exception cref="System.ArgumentNullException">query</exception>
        public async Task<QueryResult<BaseItemDto>> GetItemsAsync(ItemQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var url = GetItemListUrl(query);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        /// <summary>
        /// Gets the next up async.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Task{ItemsResult}.</returns>
        /// <exception cref="System.ArgumentNullException">query</exception>
        public async Task<QueryResult<BaseItemDto>> GetNextUpEpisodesAsync(NextUpQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var url = GetNextUpUrl(query);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetUpcomingEpisodesAsync(UpcomingEpisodesQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            if (query.Fields != null)
            {
                dict.Add("fields", query.Fields.Select(f => f.ToString()));
            }

            dict.Add("ParentId", query.ParentId);

            dict.AddIfNotNull("Limit", query.Limit);

            dict.AddIfNotNull("StartIndex", query.StartIndex);

            dict.Add("UserId", query.UserId);

            dict.AddIfNotNull("EnableImages", query.EnableImages);
            if (query.EnableImageTypes != null)
            {
                dict.Add("EnableImageTypes", query.EnableImageTypes.Select(f => f.ToString()));
            }
            dict.AddIfNotNull("ImageTypeLimit", query.ImageTypeLimit);

            var url = GetApiUrl("Shows/Upcoming", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetEpisodesAsync(EpisodeQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNull("StartIndex", query.StartIndex);
            dict.AddIfNotNull("Limit", query.Limit);

            dict.AddIfNotNullOrEmpty("StartItemId", query.StartItemId);
            dict.AddIfNotNull("Season", query.SeasonNumber);
            dict.AddIfNotNullOrEmpty("UserId", query.UserId);

            dict.AddIfNotNullOrEmpty("SeasonId", query.SeasonId);

            if (query.Fields != null)
            {
                dict.Add("Fields", query.Fields.Select(f => f.ToString()));
            }

            dict.AddIfNotNull("IsMissing", query.IsMissing);
            dict.AddIfNotNull("IsVirtualUnaired", query.IsVirtualUnaired);

            var url = GetApiUrl("Shows/" + query.SeriesId + "/Episodes", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetSeasonsAsync(SeasonQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("UserId", query.UserId);

            if (query.Fields != null)
            {
                dict.Add("Fields", query.Fields.Select(f => f.ToString()));
            }

            dict.AddIfNotNull("IsMissing", query.IsMissing);
            dict.AddIfNotNull("IsVirtualUnaired", query.IsVirtualUnaired);
            dict.AddIfNotNull("IsSpecialSeason", query.IsSpecialSeason);

            var url = GetApiUrl("Shows/" + query.SeriesId + "/Seasons", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        /// <summary>
        /// Gets the people async.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Task{ItemsResult}.</returns>
        /// <exception cref="System.ArgumentNullException">userId</exception>
        public async Task<QueryResult<BaseItemDto>> GetPeopleAsync(PersonsQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = GetItemByNameListUrl("Persons", query);

            if (query.PersonTypes != null && query.PersonTypes.Length > 0)
            {
                url += "&PersonTypes=" + string.Join(",", query.PersonTypes);
            }

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        /// <summary>
        /// Gets the genres async.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Task{ItemsResult}.</returns>
        public async Task<QueryResult<BaseItemDto>> GetGenresAsync(ItemsByNameQuery query)
        {
            var url = GetItemByNameListUrl("Genres", query);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        /// <summary>
        /// Gets the studios async.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Task{ItemsResult}.</returns>
        public async Task<QueryResult<BaseItemDto>> GetStudiosAsync(ItemsByNameQuery query)
        {
            var url = GetItemByNameListUrl("Studios", query);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        /// <summary>
        /// Gets the artists.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Task{ItemsResult}.</returns>
        /// <exception cref="System.ArgumentNullException">userId</exception>
        public async Task<QueryResult<BaseItemDto>> GetArtistsAsync(ArtistsQuery query)
        {
            var url = GetItemByNameListUrl("Artists", query);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        /// <summary>
        /// Gets the artists.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Task{ItemsResult}.</returns>
        /// <exception cref="System.ArgumentNullException">userId</exception>
        public async Task<QueryResult<BaseItemDto>> GetAlbumArtistsAsync(ArtistsQuery query)
        {
            var url = GetItemByNameListUrl("Artists/AlbumArtists", query);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        /// <summary>
        /// Restarts the server async.
        /// </summary>
        /// <returns>Task.</returns>
        public Task RestartServerAsync()
        {
            var url = GetApiUrl("System/Restart");

            return PostAsync<EmptyRequestResult>(url, new QueryStringDictionary(), CancellationToken.None);
        }

        /// <summary>
        /// Gets the system status async.
        /// </summary>
        /// <returns>Task{SystemInfo}.</returns>
        public async Task<SystemInfo> GetSystemInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = GetApiUrl("System/Info");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<SystemInfo>(stream);
            }
        }

        /// <summary>
        /// get public system information as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;PublicSystemInfo&gt;.</returns>
        public async Task<PublicSystemInfo> GetPublicSystemInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = GetApiUrl("System/Info/Public");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<PublicSystemInfo>(stream);
            }
        }

        /// <summary>
        /// Gets a list of plugins installed on the server
        /// </summary>
        /// <returns>Task{PluginInfo[]}.</returns>
        public async Task<PluginInfo[]> GetInstalledPluginsAsync()
        {
            var url = GetApiUrl("Plugins");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<PluginInfo[]>(stream);
            }
        }

        /// <summary>
        /// Gets the current server configuration
        /// </summary>
        /// <returns>Task{ServerConfiguration}.</returns>
        public async Task<ServerConfiguration> GetServerConfigurationAsync()
        {
            var url = GetApiUrl("System/Configuration");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<ServerConfiguration>(stream);
            }
        }

        /// <summary>
        /// Gets the scheduled tasks.
        /// </summary>
        /// <returns>Task{TaskInfo[]}.</returns>
        public async Task<TaskInfo[]> GetScheduledTasksAsync()
        {
            var url = GetApiUrl("ScheduledTasks");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<TaskInfo[]>(stream);
            }
        }

        /// <summary>
        /// Gets the scheduled task async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{TaskInfo}.</returns>
        /// <exception cref="System.ArgumentNullException">id</exception>
        public async Task<TaskInfo> GetScheduledTaskAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var url = GetApiUrl("ScheduledTasks/" + id);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<TaskInfo>(stream);
            }
        }

        /// <summary>
        /// Gets a user by id
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{UserDto}.</returns>
        /// <exception cref="System.ArgumentNullException">id</exception>
        public async Task<UserDto> GetUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var url = GetApiUrl("Users/" + id);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<UserDto>(stream);
            }
        }

        /// <summary>
        /// Gets the parental ratings async.
        /// </summary>
        /// <returns>Task{List{ParentalRating}}.</returns>
        public async Task<List<ParentalRating>> GetParentalRatingsAsync()
        {
            var url = GetApiUrl("Localization/ParentalRatings");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<List<ParentalRating>>(stream);
            }
        }

        /// <summary>
        /// Gets local trailers for an item
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns>Task{ItemsResult}.</returns>
        /// <exception cref="System.ArgumentNullException">query</exception>
        public async Task<BaseItemDto[]> GetLocalTrailersAsync(string userId, string itemId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentNullException("itemId");
            }

            var url = GetApiUrl("Users/" + userId + "/Items/" + itemId + "/LocalTrailers");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<BaseItemDto[]>(stream);
            }
        }

        /// <summary>
        /// Gets special features for an item
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns>Task{BaseItemDto[]}.</returns>
        /// <exception cref="System.ArgumentNullException">userId</exception>
        public async Task<BaseItemDto[]> GetSpecialFeaturesAsync(string userId, string itemId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentNullException("itemId");
            }

            var url = GetApiUrl("Users/" + userId + "/Items/" + itemId + "/SpecialFeatures");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<BaseItemDto[]>(stream);
            }
        }

        /// <summary>
        /// Gets the cultures async.
        /// </summary>
        /// <returns>Task{CultureDto[]}.</returns>
        public async Task<CultureDto[]> GetCulturesAsync()
        {
            var url = GetApiUrl("Localization/Cultures");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<CultureDto[]>(stream);
            }
        }

        /// <summary>
        /// Gets the countries async.
        /// </summary>
        /// <returns>Task{CountryInfo[]}.</returns>
        public async Task<CountryInfo[]> GetCountriesAsync()
        {
            var url = GetApiUrl("Localization/Countries");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<CountryInfo[]>(stream);
            }
        }

        /// <summary>
        /// Gets the game system summaries async.
        /// </summary>
        /// <returns>Task{List{GameSystemSummary}}.</returns>
        public async Task<List<GameSystemSummary>> GetGameSystemSummariesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = GetApiUrl("Games/SystemSummaries");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<List<GameSystemSummary>>(stream);
            }
        }

        public Task<UserItemDataDto> MarkPlayedAsync(string itemId, string userId, DateTime? datePlayed)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentNullException("itemId");
            }
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var dict = new QueryStringDictionary();

            if (datePlayed.HasValue)
            {
                dict.Add("DatePlayed", datePlayed.Value.ToString("yyyyMMddHHmmss"));
            }

            var url = GetApiUrl("Users/" + userId + "/PlayedItems/" + itemId, dict);

            return PostAsync<UserItemDataDto>(url, new Dictionary<string, string>(), CancellationToken.None);
        }

        /// <summary>
        /// Marks the unplayed async.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Task{UserItemDataDto}.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// itemId
        /// or
        /// userId
        /// </exception>
        public Task<UserItemDataDto> MarkUnplayedAsync(string itemId, string userId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentNullException("itemId");
            }
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var url = GetApiUrl("Users/" + userId + "/PlayedItems/" + itemId);

            return DeleteAsync<UserItemDataDto>(url, CancellationToken.None);
        }

        /// <summary>
        /// Updates the favorite status async.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="isFavorite">if set to <c>true</c> [is favorite].</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">itemId</exception>
        public Task<UserItemDataDto> UpdateFavoriteStatusAsync(string itemId, string userId, bool isFavorite)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentNullException("itemId");
            }
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var url = GetApiUrl("Users/" + userId + "/FavoriteItems/" + itemId);

            if (isFavorite)
            {
                return PostAsync<UserItemDataDto>(url, new Dictionary<string, string>(), CancellationToken.None);
            }

            return DeleteAsync<UserItemDataDto>(url, CancellationToken.None);
        }

        /// <summary>
        /// Reports to the server that the user has begun playing an item
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task{UserItemDataDto}.</returns>
        /// <exception cref="System.ArgumentNullException">itemId</exception>
        public Task ReportPlaybackStartAsync(PlaybackStartInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Logger.Debug("ReportPlaybackStart: Item {0}", info.ItemId);

            var url = GetApiUrl("Sessions/Playing");

            return PostAsync<PlaybackStartInfo, EmptyRequestResult>(url, info, CancellationToken.None);
        }

        /// <summary>
        /// Reports playback progress to the server
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task{UserItemDataDto}.</returns>
        /// <exception cref="System.ArgumentNullException">itemId</exception>
        public Task ReportPlaybackProgressAsync(PlaybackProgressInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            if (IsWebSocketConnected)
            {
                return SendWebSocketMessage("ReportPlaybackProgress", JsonSerializer.SerializeToString(info));
            }

            var url = GetApiUrl("Sessions/Playing/Progress");

            return PostAsync<PlaybackProgressInfo, EmptyRequestResult>(url, info, CancellationToken.None);
        }

        /// <summary>
        /// Reports to the server that the user has stopped playing an item
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task{UserItemDataDto}.</returns>
        /// <exception cref="System.ArgumentNullException">itemId</exception>
        public Task ReportPlaybackStoppedAsync(PlaybackStopInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            var url = GetApiUrl("Sessions/Playing/Stopped");

            return PostAsync<PlaybackStopInfo, EmptyRequestResult>(url, info, CancellationToken.None);
        }

        /// <summary>
        /// Instructs antoher client to browse to a library item.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="itemId">The id of the item to browse to.</param>
        /// <param name="itemName">The name of the item to browse to.</param>
        /// <param name="itemType">The type of the item to browse to.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">sessionId
        /// or
        /// itemId
        /// or
        /// itemName
        /// or
        /// itemType</exception>
        public Task SendBrowseCommandAsync(string sessionId, string itemId, string itemName, string itemType)
        {
            var cmd = new GeneralCommand
            {
                Name = "DisplayContent"
            };

            cmd.Arguments["ItemType"] = itemType;
            cmd.Arguments["ItemId"] = itemId;
            cmd.Arguments["ItemName"] = itemName;

            return SendCommandAsync(sessionId, cmd);
        }

        /// <summary>
        /// Sends the play command async.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">sessionId
        /// or
        /// request</exception>
        public Task SendPlayCommandAsync(string sessionId, PlayRequest request)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentNullException("sessionId");
            }
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var dict = new QueryStringDictionary();
            dict.Add("ItemIds", request.ItemIds);
            dict.AddIfNotNull("StartPositionTicks", request.StartPositionTicks);
            dict.Add("PlayCommand", request.PlayCommand.ToString());

            var url = GetApiUrl("Sessions/" + sessionId + "/Playing", dict);

            return PostAsync<EmptyRequestResult>(url, new Dictionary<string, string>(), CancellationToken.None);
        }

        public Task SendMessageCommandAsync(string sessionId, MessageCommand command)
        {
            var cmd = new GeneralCommand
            {
                Name = "DisplayMessage"
            };

            cmd.Arguments["Header"] = command.Header;
            cmd.Arguments["Text"] = command.Text;

            if (command.TimeoutMs.HasValue)
            {
                cmd.Arguments["Timeout"] = command.TimeoutMs.Value.ToString(CultureInfo.InvariantCulture);
            }

            return SendCommandAsync(sessionId, cmd);
        }

        /// <summary>
        /// Sends the system command async.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="command">The command.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">sessionId</exception>
        public Task SendCommandAsync(string sessionId, GeneralCommand command)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentNullException("sessionId");
            }

            var url = GetApiUrl("Sessions/" + sessionId + "/Command");

            return PostAsync<GeneralCommand, EmptyRequestResult>(url, command, CancellationToken.None);
        }

        /// <summary>
        /// Sends the playstate command async.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task.</returns>
        public Task SendPlaystateCommandAsync(string sessionId, PlaystateRequest request)
        {
            var dict = new QueryStringDictionary();
            dict.AddIfNotNull("SeekPositionTicks", request.SeekPositionTicks);

            var url = GetApiUrl("Sessions/" + sessionId + "/Playing/" + request.Command.ToString(), dict);

            return PostAsync<EmptyRequestResult>(url, new Dictionary<string, string>(), CancellationToken.None);
        }

        /// <summary>
        /// Clears a user's rating for an item
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>Task{UserItemDataDto}.</returns>
        /// <exception cref="System.ArgumentNullException">itemId</exception>
        public Task<UserItemDataDto> ClearUserItemRatingAsync(string itemId, string userId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentNullException("itemId");
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var url = GetApiUrl("Users/" + userId + "/Items/" + itemId + "/Rating");

            return DeleteAsync<UserItemDataDto>(url, CancellationToken.None);
        }

        /// <summary>
        /// Updates a user's rating for an item, based on likes or dislikes
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="likes">if set to <c>true</c> [likes].</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">itemId</exception>
        public Task<UserItemDataDto> UpdateUserItemRatingAsync(string itemId, string userId, bool likes)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentNullException("itemId");
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var dict = new QueryStringDictionary { };

            dict.Add("likes", likes);

            var url = GetApiUrl("Users/" + userId + "/Items/" + itemId + "/Rating", dict);

            return PostAsync<UserItemDataDto>(url, new Dictionary<string, string>(), CancellationToken.None);
        }

        internal Func<IApiClient, AuthenticationResult, Task> OnAuthenticated { get; set; }

        /// <summary>
        /// Authenticates a user and returns the result
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentNullException">username</exception>
        /// <exception cref="System.ArgumentNullException">userId</exception>
        public async Task<AuthenticationResult> AuthenticateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("username");
            }

            var url = GetApiUrl("Users/AuthenticateByName");

            var args = new Dictionary<string, string>();

            args["username"] = Uri.EscapeDataString(username);
            args["pw"] = password;

            var bytes = Encoding.UTF8.GetBytes(password ?? string.Empty);
            args["password"] = BitConverter.ToString(_cryptographyProvider.CreateSha1(bytes)).Replace("-", string.Empty);

            args["passwordMD5"] = ConnectService.GetConnectPasswordMd5(password ?? string.Empty, _cryptographyProvider);

            var result = await PostAsync<AuthenticationResult>(url, args, CancellationToken.None);

            SetAuthenticationInfo(result.AccessToken, result.User.Id);

            if (OnAuthenticated != null)
            {
                await OnAuthenticated(this, result).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Updates the server configuration async.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public Task UpdateServerConfigurationAsync(ServerConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            var url = GetApiUrl("System/Configuration");

            return PostAsync<ServerConfiguration, EmptyRequestResult>(url, configuration, CancellationToken.None);
        }

        /// <summary>
        /// Updates the scheduled task triggers.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="triggers">The triggers.</param>
        /// <returns>Task{RequestResult}.</returns>
        /// <exception cref="System.ArgumentNullException">id</exception>
        public Task UpdateScheduledTaskTriggersAsync(string id, TaskTriggerInfo[] triggers)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            if (triggers == null)
            {
                throw new ArgumentNullException("triggers");
            }

            var url = GetApiUrl("ScheduledTasks/" + id + "/Triggers");

            return PostAsync<TaskTriggerInfo[], EmptyRequestResult>(url, triggers, CancellationToken.None);
        }

        /// <summary>
        /// Gets the display preferences.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="client">The client.</param>
        /// <returns>Task{BaseItemDto}.</returns>
        public async Task<DisplayPreferences> GetDisplayPreferencesAsync(string id, string userId, string client, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dict = new QueryStringDictionary();

            dict.Add("userId", userId);
            dict.Add("client", client);

            var url = GetApiUrl("DisplayPreferences/" + id, dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<DisplayPreferences>(stream);
            }
        }

        /// <summary>
        /// Updates display preferences for a user
        /// </summary>
        /// <param name="displayPreferences">The display preferences.</param>
        /// <returns>Task{DisplayPreferences}.</returns>
        /// <exception cref="System.ArgumentNullException">userId</exception>
        public Task UpdateDisplayPreferencesAsync(DisplayPreferences displayPreferences, string userId, string client, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (displayPreferences == null)
            {
                throw new ArgumentNullException("displayPreferences");
            }

            var dict = new QueryStringDictionary();

            dict.Add("userId", userId);
            dict.Add("client", client);

            var url = GetApiUrl("DisplayPreferences/" + displayPreferences.Id, dict);

            return PostAsync<DisplayPreferences, EmptyRequestResult>(url, displayPreferences, cancellationToken);
        }

        /// <summary>
        /// Posts a set of data to a url, and deserializes the return stream into T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="args">The args.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{``0}.</returns>
        public async Task<T> PostAsync<T>(string url, Dictionary<string, string> args, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            url = AddDataFormat(url);

            // Create the post body
            var strings = args.Keys.Select(key => string.Format("{0}={1}", key, args[key]));
            var postContent = string.Join("&", strings.ToArray());

            const string contentType = "application/x-www-form-urlencoded";

            using (var stream = await SendAsync(new HttpRequest
            {
                Url = url,
                CancellationToken = cancellationToken,
                RequestHeaders = HttpHeaders,
                Method = "POST",
                RequestContentType = contentType,
                RequestContent = postContent
            }).ConfigureAwait(false))
            {
                return DeserializeFromStream<T>(stream);
            }
        }

        /// <summary>
        /// Deletes the async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{``0}.</returns>
        private async Task<T> DeleteAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            url = AddDataFormat(url);

            using (var stream = await SendAsync(new HttpRequest
            {
                Url = url,
                CancellationToken = cancellationToken,
                RequestHeaders = HttpHeaders,
                Method = "DELETE"

            }).ConfigureAwait(false))
            {
                return DeserializeFromStream<T>(stream);
            }
        }

        /// <summary>
        /// Posts an object of type TInputType to a given url, and deserializes the response into an object of type TOutputType
        /// </summary>
        /// <typeparam name="TInputType">The type of the T input type.</typeparam>
        /// <typeparam name="TOutputType">The type of the T output type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{``1}.</returns>
        private async Task<TOutputType> PostAsync<TInputType, TOutputType>(string url, TInputType obj, CancellationToken cancellationToken = default(CancellationToken))
            where TOutputType : class
        {
            url = AddDataFormat(url);

            const string contentType = "application/json";

            var postContent = SerializeToJson(obj);

            using (var stream = await SendAsync(new HttpRequest
            {
                Url = url,
                CancellationToken = cancellationToken,
                RequestHeaders = HttpHeaders,
                Method = "POST",
                RequestContentType = contentType,
                RequestContent = postContent
            }).ConfigureAwait(false))
            {
                return DeserializeFromStream<TOutputType>(stream);
            }
        }

        /// <summary>
        /// This is a helper around getting a stream from the server that contains serialized data
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{Stream}.</returns>
        public Task<Stream> GetSerializedStreamAsync(string url, CancellationToken cancellationToken)
        {
            url = AddDataFormat(url);

            return GetStream(url, cancellationToken);
        }

        public Task<Stream> GetSerializedStreamAsync(string url)
        {
            return GetSerializedStreamAsync(url, CancellationToken.None);
        }

        public async Task<NotificationsSummary> GetNotificationsSummary(string userId)
        {
            var url = GetApiUrl("Notifications/" + userId + "/Summary");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<NotificationsSummary>(stream);
            }
        }

        public Task MarkNotificationsRead(string userId, IEnumerable<string> notificationIdList, bool isRead)
        {
            var url = "Notifications/" + userId;

            url += isRead ? "/Read" : "/Unread";

            var dict = new QueryStringDictionary();

            var ids = notificationIdList.ToArray();

            dict.Add("Ids", string.Join(",", ids));

            url = GetApiUrl(url, dict);

            return PostAsync<EmptyRequestResult>(url, new Dictionary<string, string>(), CancellationToken.None);
        }

        public async Task<NotificationResult> GetNotificationsAsync(NotificationQuery query)
        {
            var url = "Notifications/" + query.UserId;

            var dict = new QueryStringDictionary();
            dict.AddIfNotNull("ItemIds", query.IsRead);
            dict.AddIfNotNull("StartIndex", query.StartIndex);
            dict.AddIfNotNull("Limit", query.Limit);

            url = GetApiUrl(url, dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<NotificationResult>(stream);
            }
        }

        public async Task<AllThemeMediaResult> GetAllThemeMediaAsync(string userId, string itemId, bool inheritFromParent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryString = new QueryStringDictionary();

            queryString.Add("InheritFromParent", inheritFromParent);
            queryString.AddIfNotNullOrEmpty("UserId", userId);

            var url = GetApiUrl("Items/" + itemId + "/ThemeMedia", queryString);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<AllThemeMediaResult>(stream);
            }
        }

        public async Task<SearchHintResult> GetSearchHintsAsync(SearchQuery query)
        {
            if (query == null || string.IsNullOrEmpty(query.SearchTerm))
            {
                throw new ArgumentNullException("query");
            }

            var queryString = new QueryStringDictionary();

            queryString.AddIfNotNullOrEmpty("SearchTerm", query.SearchTerm);
            queryString.AddIfNotNullOrEmpty("UserId", query.UserId);
            queryString.AddIfNotNull("StartIndex", query.StartIndex);
            queryString.AddIfNotNull("Limit", query.Limit);

            queryString.Add("IncludeArtists", query.IncludeArtists);
            queryString.Add("IncludeGenres", query.IncludeGenres);
            queryString.Add("IncludeMedia", query.IncludeMedia);
            queryString.Add("IncludePeople", query.IncludePeople);
            queryString.Add("IncludeStudios", query.IncludeStudios);

            var url = GetApiUrl("Search/Hints", queryString);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<SearchHintResult>(stream);
            }
        }

        public async Task<ThemeMediaResult> GetThemeSongsAsync(string userId, string itemId, bool inheritFromParent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryString = new QueryStringDictionary();

            queryString.Add("InheritFromParent", inheritFromParent);
            queryString.AddIfNotNullOrEmpty("UserId", userId);

            var url = GetApiUrl("Items/" + itemId + "/ThemeSongs", queryString);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<ThemeMediaResult>(stream);
            }
        }

        public async Task<ThemeMediaResult> GetThemeVideosAsync(string userId, string itemId, bool inheritFromParent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryString = new QueryStringDictionary();

            queryString.Add("InheritFromParent", inheritFromParent);
            queryString.AddIfNotNullOrEmpty("UserId", userId);

            var url = GetApiUrl("Items/" + itemId + "/ThemeVideos", queryString);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<ThemeMediaResult>(stream);
            }
        }

        /// <summary>
        /// Gets the critic reviews.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task{ItemReviewsResult}.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// id
        /// or
        /// userId
        /// </exception>
        public async Task<QueryResult<ItemReview>> GetCriticReviews(string itemId, CancellationToken cancellationToken = default(CancellationToken), int? startIndex = null, int? limit = null)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                throw new ArgumentNullException("itemId");
            }

            var queryString = new QueryStringDictionary();

            queryString.AddIfNotNull("startIndex", startIndex);
            queryString.AddIfNotNull("limit", limit);

            var url = GetApiUrl("Items/" + itemId + "/CriticReviews", queryString);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<ItemReview>>(stream);
            }
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<T>(stream);
            }
        }

        /// <summary>
        /// Gets the index of the game player.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{List{ItemIndex}}.</returns>
        public async Task<List<ItemIndex>> GetGamePlayerIndex(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryString = new QueryStringDictionary();

            queryString.AddIfNotNullOrEmpty("UserId", userId);

            var url = GetApiUrl("Games/PlayerIndex", queryString);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<List<ItemIndex>>(stream);
            }
        }

        /// <summary>
        /// Gets the index of the year.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="includeItemTypes">The include item types.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{List{ItemIndex}}.</returns>
        public async Task<List<ItemIndex>> GetYearIndex(string userId, string[] includeItemTypes, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryString = new QueryStringDictionary();

            queryString.AddIfNotNullOrEmpty("UserId", userId);
            queryString.AddIfNotNull("IncludeItemTypes", includeItemTypes);

            var url = GetApiUrl("Items/YearIndex", queryString);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<List<ItemIndex>>(stream);
            }
        }

        public Task ReportCapabilities(ClientCapabilities capabilities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (capabilities == null)
            {
                throw new ArgumentNullException("capabilities");
            }

            var url = GetApiUrl("Sessions/Capabilities/Full");

            return PostAsync<ClientCapabilities, EmptyRequestResult>(url, capabilities, cancellationToken);
        }

        public async Task<LiveTvInfo> GetLiveTvInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = GetApiUrl("LiveTv/Info");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<LiveTvInfo>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetLiveTvRecordingGroupsAsync(RecordingGroupQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("UserId", query.UserId);

            var url = GetApiUrl("LiveTv/Recordings/Groups", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetLiveTvRecordingsAsync(RecordingQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("UserId", query.UserId);
            dict.AddIfNotNullOrEmpty("ChannelId", query.ChannelId);
            dict.AddIfNotNullOrEmpty("GroupId", query.GroupId);
            dict.AddIfNotNullOrEmpty("Id", query.Id);
            dict.AddIfNotNullOrEmpty("SeriesTimerId", query.SeriesTimerId);
            dict.AddIfNotNull("IsInProgress", query.IsInProgress);
            dict.AddIfNotNull("StartIndex", query.StartIndex);
            dict.AddIfNotNull("Limit", query.Limit);

            if (!query.EnableTotalRecordCount)
            {
                dict.Add("EnableTotalRecordCount", query.EnableTotalRecordCount);
            }

            var url = GetApiUrl("LiveTv/Recordings", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<QueryResult<ChannelInfoDto>> GetLiveTvChannelsAsync(LiveTvChannelQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("UserId", query.UserId);
            dict.AddIfNotNull("StartIndex", query.StartIndex);
            dict.AddIfNotNull("Limit", query.Limit);
            dict.AddIfNotNull("IsFavorite", query.IsFavorite);
            dict.AddIfNotNull("IsLiked", query.IsLiked);
            dict.AddIfNotNull("IsDisliked", query.IsDisliked);
            dict.AddIfNotNull("EnableFavoriteSorting", query.EnableFavoriteSorting);


            if (query.ChannelType.HasValue)
            {
                dict.Add("ChannelType", query.ChannelType.Value.ToString());
            }

            var url = GetApiUrl("LiveTv/Channels", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<ChannelInfoDto>>(stream);
            }
        }

        public Task CancelLiveTvSeriesTimerAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var dict = new QueryStringDictionary { };

            var url = GetApiUrl("LiveTv/SeriesTimers/" + id, dict);

            return SendAsync(new HttpRequest
            {
                Url = url,
                CancellationToken = cancellationToken,
                RequestHeaders = HttpHeaders,
                Method = "DELETE"
            });
        }

        public Task CancelLiveTvTimerAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var dict = new QueryStringDictionary { };

            var url = GetApiUrl("LiveTv/Timers/" + id, dict);

            return SendAsync(new HttpRequest
            {
                Url = url,
                CancellationToken = cancellationToken,
                RequestHeaders = HttpHeaders,
                Method = "DELETE"
            });
        }

        public async Task<ChannelInfoDto> GetLiveTvChannelAsync(string id, string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var dict = new QueryStringDictionary { };
            dict.AddIfNotNullOrEmpty("userId", userId);

            var url = GetApiUrl("LiveTv/Channels/" + id, dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<ChannelInfoDto>(stream);
            }
        }

        public async Task<BaseItemDto> GetLiveTvRecordingAsync(string id, string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var dict = new QueryStringDictionary { };
            dict.AddIfNotNullOrEmpty("userId", userId);

            var url = GetApiUrl("LiveTv/Recordings/" + id, dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<BaseItemDto>(stream);
            }
        }

        public async Task<BaseItemDto> GetLiveTvRecordingGroupAsync(string id, string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var dict = new QueryStringDictionary { };
            dict.AddIfNotNullOrEmpty("userId", userId);

            var url = GetApiUrl("LiveTv/Recordings/Groups/" + id, dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<BaseItemDto>(stream);
            }
        }

        public async Task<SeriesTimerInfoDto> GetLiveTvSeriesTimerAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var dict = new QueryStringDictionary { };

            var url = GetApiUrl("LiveTv/SeriesTimers/" + id, dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<SeriesTimerInfoDto>(stream);
            }
        }

        public async Task<QueryResult<SeriesTimerInfoDto>> GetLiveTvSeriesTimersAsync(SeriesTimerQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("SortBy", query.SortBy);
            dict.Add("SortOrder", query.SortOrder.ToString());

            var url = GetApiUrl("LiveTv/SeriesTimers", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<SeriesTimerInfoDto>>(stream);
            }
        }

        public async Task<TimerInfoDto> GetLiveTvTimerAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var dict = new QueryStringDictionary { };

            var url = GetApiUrl("LiveTv/Timers/" + id, dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<TimerInfoDto>(stream);
            }
        }

        public async Task<QueryResult<TimerInfoDto>> GetLiveTvTimersAsync(TimerQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("ChannelId", query.ChannelId);
            dict.AddIfNotNullOrEmpty("SeriesTimerId", query.SeriesTimerId);

            var url = GetApiUrl("LiveTv/Timers", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<TimerInfoDto>>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetLiveTvProgramsAsync(ProgramQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            const string isoDateFormat = "o";

            if (query.MaxEndDate.HasValue)
            {
                dict.Add("MaxEndDate", query.MaxEndDate.Value.ToUniversalTime().ToString(isoDateFormat));
            }
            if (query.MaxStartDate.HasValue)
            {
                dict.Add("MaxStartDate", query.MaxStartDate.Value.ToUniversalTime().ToString(isoDateFormat));
            }
            if (query.MinEndDate.HasValue)
            {
                dict.Add("MinEndDate", query.MinEndDate.Value.ToUniversalTime().ToString(isoDateFormat));
            }
            if (query.MinStartDate.HasValue)
            {
                dict.Add("MinStartDate", query.MinStartDate.Value.ToUniversalTime().ToString(isoDateFormat));
            }

            dict.AddIfNotNullOrEmpty("UserId", query.UserId);

            if (!query.EnableTotalRecordCount)
            {
                dict.Add("EnableTotalRecordCount", query.EnableTotalRecordCount);
            }

            if (query.ChannelIds != null)
            {
                dict.Add("ChannelIds", string.Join(",", query.ChannelIds));
            }

            // TODO: This endpoint supports POST if the query string is too long
            var url = GetApiUrl("LiveTv/Programs", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetRecommendedLiveTvProgramsAsync(RecommendedProgramQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("UserId", query.UserId);
            dict.AddIfNotNull("Limit", query.Limit);
            dict.AddIfNotNull("HasAired", query.HasAired);
            dict.AddIfNotNull("IsAiring", query.IsAiring);

            if (!query.EnableTotalRecordCount)
            {
                dict.Add("EnableTotalRecordCount", query.EnableTotalRecordCount);
            }

            var url = GetApiUrl("LiveTv/Programs/Recommended", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public Task CreateLiveTvSeriesTimerAsync(SeriesTimerInfoDto timer, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (timer == null)
            {
                throw new ArgumentNullException("timer");
            }

            var url = GetApiUrl("LiveTv/SeriesTimers");

            return PostAsync<SeriesTimerInfoDto, EmptyRequestResult>(url, timer, cancellationToken);
        }

        public Task CreateLiveTvTimerAsync(BaseTimerInfoDto timer, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (timer == null)
            {
                throw new ArgumentNullException("timer");
            }

            var url = GetApiUrl("LiveTv/Timers");

            return PostAsync<BaseTimerInfoDto, EmptyRequestResult>(url, timer, cancellationToken);
        }

        public async Task<SeriesTimerInfoDto> GetDefaultLiveTvTimerInfo(string programId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(programId))
            {
                throw new ArgumentNullException("programId");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("programId", programId);

            var url = GetApiUrl("LiveTv/Timers/Defaults", dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<SeriesTimerInfoDto>(stream);
            }
        }

        public async Task<SeriesTimerInfoDto> GetDefaultLiveTvTimerInfo(CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = GetApiUrl("LiveTv/Timers/Defaults");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<SeriesTimerInfoDto>(stream);
            }
        }

        public async Task<GuideInfo> GetLiveTvGuideInfo(CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = GetApiUrl("LiveTv/GuideInfo");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<GuideInfo>(stream);
            }
        }

        public async Task<BaseItemDto> GetLiveTvProgramAsync(string id, string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var dict = new QueryStringDictionary { };
            dict.AddIfNotNullOrEmpty("userId", userId);

            var url = GetApiUrl("LiveTv/Programs/" + id, dict);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<BaseItemDto>(stream);
            }
        }

        public Task UpdateLiveTvSeriesTimerAsync(SeriesTimerInfoDto timer, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (timer == null)
            {
                throw new ArgumentNullException("timer");
            }

            var url = GetApiUrl("LiveTv/SeriesTimers/" + timer.Id);

            return PostAsync<SeriesTimerInfoDto, EmptyRequestResult>(url, timer, cancellationToken);
        }

        public Task UpdateLiveTvTimerAsync(TimerInfoDto timer, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (timer == null)
            {
                throw new ArgumentNullException("timer");
            }

            var url = GetApiUrl("LiveTv/Timers/" + timer.Id);

            return PostAsync<TimerInfoDto, EmptyRequestResult>(url, timer, cancellationToken);
        }

        public Task SendString(string sessionId, string text)
        {
            var cmd = new GeneralCommand
            {
                Name = "SendString"
            };

            cmd.Arguments["String"] = text;

            return SendCommandAsync(sessionId, cmd);
        }

        public Task SetAudioStreamIndex(string sessionId, int index)
        {
            var cmd = new GeneralCommand
            {
                Name = "SetAudioStreamIndex"
            };

            cmd.Arguments["Index"] = index.ToString(CultureInfo.InvariantCulture);

            return SendCommandAsync(sessionId, cmd);
        }

        public Task SetSubtitleStreamIndex(string sessionId, int? index)
        {
            var cmd = new GeneralCommand
            {
                Name = "SetSubtitleStreamIndex"
            };

            cmd.Arguments["Index"] = (index ?? -1).ToString(CultureInfo.InvariantCulture);

            return SendCommandAsync(sessionId, cmd);
        }

        public Task SetVolume(string sessionId, int volume)
        {
            var cmd = new GeneralCommand
            {
                Name = "SetVolume"
            };

            cmd.Arguments["Volume"] = volume.ToString(CultureInfo.InvariantCulture);

            return SendCommandAsync(sessionId, cmd);
        }

        public async Task<QueryResult<BaseItemDto>> GetAdditionalParts(string itemId, string userId)
        {
            var queryString = new QueryStringDictionary();

            queryString.AddIfNotNullOrEmpty("UserId", userId);

            var url = GetApiUrl("Videos/" + itemId + "/AdditionalParts", queryString);

            using (var stream = await GetSerializedStreamAsync(url, CancellationToken.None).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<ChannelFeatures> GetChannelFeatures(string channelId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = GetApiUrl("Channels/" + channelId + "/Features");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<ChannelFeatures>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetChannelItems(ChannelItemQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryString = new QueryStringDictionary();

            queryString.AddIfNotNullOrEmpty("UserId", query.UserId);
            queryString.AddIfNotNull("StartIndex", query.StartIndex);
            queryString.AddIfNotNull("Limit", query.Limit);
            queryString.AddIfNotNullOrEmpty("FolderId", query.FolderId);
            if (query.Fields != null)
            {
                queryString.Add("fields", query.Fields.Select(f => f.ToString()));
            }
            if (query.Filters != null)
            {
                queryString.Add("Filters", query.Filters.Select(f => f.ToString()));
            }

            var sortBy = new List<string>();
            var sortOrder = new List<string>();
            foreach (var order in query.OrderBy)
            {
                sortBy.Add(order.Item1);
                sortOrder.Add(order.Item2.ToString());
            }

            queryString.AddIfNotNull("SortBy", sortBy);
            queryString.AddIfNotNull("SortOrder", sortOrder);

            var url = GetApiUrl("Channels/" + query.ChannelId + "/Items", queryString);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetChannels(ChannelQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryString = new QueryStringDictionary();

            queryString.AddIfNotNullOrEmpty("UserId", query.UserId);
            queryString.AddIfNotNull("SupportsLatestItems", query.SupportsLatestItems);
            queryString.AddIfNotNull("StartIndex", query.StartIndex);
            queryString.AddIfNotNull("Limit", query.Limit);
            queryString.AddIfNotNull("IsFavorite", query.IsFavorite);

            var url = GetApiUrl("Channels", queryString);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<SessionInfoDto> GetCurrentSessionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryString = new QueryStringDictionary();

            queryString.Add("DeviceId", DeviceId);
            var url = GetApiUrl("Sessions", queryString);

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                var sessions = DeserializeFromStream<SessionInfoDto[]>(stream);

                return sessions.FirstOrDefault();
            }
        }

        public Task StopTranscodingProcesses(string deviceId, string playSessionId)
        {
            var queryString = new QueryStringDictionary();

            queryString.Add("DeviceId", DeviceId);
            queryString.AddIfNotNullOrEmpty("PlaySessionId", playSessionId);
            var url = GetApiUrl("Videos/ActiveEncodings", queryString);

            return SendAsync(new HttpRequest
            {
                Url = url,
                RequestHeaders = HttpHeaders,
                Method = "DELETE"
            });
        }

        public async Task<QueryResult<BaseItemDto>> GetLatestChannelItems(AllChannelMediaQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (string.IsNullOrEmpty(query.UserId))
            {
                throw new ArgumentNullException("userId");
            }

            var queryString = new QueryStringDictionary();
            queryString.Add("UserId", query.UserId);
            queryString.AddIfNotNull("StartIndex", query.StartIndex);
            queryString.AddIfNotNull("Limit", query.Limit);
            if (query.Filters != null)
            {
                queryString.Add("Filters", query.Filters.Select(f => f.ToString()));
            }
            if (query.Fields != null)
            {
                queryString.Add("Fields", query.Fields.Select(f => f.ToString()));
            }

            queryString.AddIfNotNull("ChannelIds", query.ChannelIds);

            var url = GetApiUrl("Channels/Items/Latest");

            using (var stream = await GetSerializedStreamAsync(url, CancellationToken.None).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task Logout()
        {
            try
            {
                var url = GetApiUrl("Sessions/Logout");

                await PostAsync<EmptyRequestResult>(url, new Dictionary<string, string>(), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Error logging out", ex);
            }

            ClearAuthenticationInfo();
        }

        public async Task<QueryResult<BaseItemDto>> GetUserViews(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            var url = GetApiUrl("Users/" + userId + "/Views");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                var result = DeserializeFromStream<QueryResult<BaseItemDto>>(stream);

                var serverInfo = ServerInfo;
                if (serverInfo != null && _localAssetManager != null)
                {
                    var offlineView = await GetOfflineView(serverInfo.Id, userId).ConfigureAwait(false);

                    if (offlineView != null)
                    {
                        var list = result.Items.ToList();
                        list.Add(offlineView);
                        result.Items = list.ToArray();
                        result.TotalRecordCount = list.Count;
                    }
                }

                return result;
            }
        }

        public async Task<BaseItemDto[]> GetLatestItems(LatestItemsQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (string.IsNullOrEmpty(query.UserId))
            {
                throw new ArgumentNullException("userId");
            }

            var queryString = new QueryStringDictionary();
            queryString.AddIfNotNull("GroupItems", query.GroupItems);
            queryString.AddIfNotNull("IncludeItemTypes", query.IncludeItemTypes);
            queryString.AddIfNotNullOrEmpty("ParentId", query.ParentId);
            queryString.AddIfNotNull("IsPlayed", query.IsPlayed);
            queryString.AddIfNotNull("StartIndex", query.StartIndex);
            queryString.AddIfNotNull("Limit", query.Limit);

            if (query.Fields != null)
            {
                queryString.Add("fields", query.Fields.Select(f => f.ToString()));
            }

            var url = GetApiUrl("Users/" + query.UserId + "/Items/Latest", queryString);

            using (var stream = await GetSerializedStreamAsync(url, CancellationToken.None).ConfigureAwait(false))
            {
                return DeserializeFromStream<BaseItemDto[]>(stream);
            }
        }

        private async Task<BaseItemDto> GetOfflineView(string serverId, string userId)
        {
            var views = await _localAssetManager.GetViews(serverId, userId).ConfigureAwait(false);

            if (views.Count > 0)
            {
                return new BaseItemDto
                {
                    Name = "AnyTime",
                    ServerId = serverId,
                    Id = "OfflineView",
                    Type = "OfflineView"
                };
            }

            return null;
        }

        public Task AddToPlaylist(string playlistId, IEnumerable<string> itemIds, string userId)
        {
            if (playlistId == null)
            {
                throw new ArgumentNullException("playlistId");
            }

            if (itemIds == null)
            {
                throw new ArgumentNullException("itemIds");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNull("Ids", itemIds);
            var url = GetApiUrl(string.Format("Playlists/{0}/Items", playlistId), dict);
            return PostAsync<EmptyRequestResult>(url, new Dictionary<string, string>(), CancellationToken.None);
        }

        public async Task<PlaylistCreationResult> CreatePlaylist(PlaylistCreationRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                throw new ArgumentNullException("userId");
            }

            if (string.IsNullOrEmpty(request.MediaType) && (request.ItemIdList == null || !request.ItemIdList.Any()))
            {
                throw new ArgumentNullException("must provide either MediaType or Ids");
            }

            var queryString = new QueryStringDictionary();

            queryString.Add("UserId", request.UserId);
            queryString.Add("Name", request.Name);

            if (!string.IsNullOrEmpty(request.MediaType))
                queryString.Add("MediaType", request.MediaType);

            if (request.ItemIdList != null && request.ItemIdList.Any())
                queryString.Add("Ids", request.ItemIdList);

            var url = GetApiUrl("Playlists/", queryString);

            return await PostAsync<PlaylistCreationResult>(url, new Dictionary<string, string>(), CancellationToken.None);

        }

        public async Task<QueryResult<BaseItemDto>> GetPlaylistItems(PlaylistItemQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNull("StartIndex", query.StartIndex);

            dict.AddIfNotNull("Limit", query.Limit);
            dict.Add("UserId", query.UserId);

            if (query.Fields != null)
            {
                dict.Add("fields", query.Fields.Select(f => f.ToString()));
            }

            var url = GetApiUrl("Playlists/" + query.Id + "/Items", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public Task RemoveFromPlaylist(string playlistId, IEnumerable<string> entryIds)
        {
            if (playlistId == null)
            {
                throw new ArgumentNullException("playlistId");
            }

            if (entryIds == null)
            {
                throw new ArgumentNullException("entryIds");
            }

            var dict = new QueryStringDictionary { };

            dict.AddIfNotNull("EntryIds", entryIds);
            var url = GetApiUrl(string.Format("Playlists/{0}/Items", playlistId), dict);
            return DeleteAsync<EmptyRequestResult>(url, CancellationToken.None);
        }

        public async Task<ContentUploadHistory> GetContentUploadHistory(string deviceId)
        {
            var dict = new QueryStringDictionary { };

            dict.Add("DeviceId", deviceId);

            var url = GetApiUrl("Devices/CameraUploads", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<ContentUploadHistory>(stream);
            }
        }

        public async Task UploadFile(Stream stream, LocalFileInfo file, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dict = new QueryStringDictionary { };

            dict.Add("DeviceId", DeviceId);
            dict.Add("Name", file.Name);
            dict.Add("Id", file.Id);
            dict.AddIfNotNullOrEmpty("Album", file.Album);

            var url = GetApiUrl("Devices/CameraUploads", dict);

            using (stream)
            {
                await SendAsync(new HttpRequest
                {
                    CancellationToken = cancellationToken,
                    Method = "POST",
                    RequestHeaders = HttpHeaders,
                    Url = url,
                    RequestContentType = file.MimeType,
                    RequestStream = stream

                }, false).ConfigureAwait(false);
            }
        }

        public async Task<DevicesOptions> GetDevicesOptions()
        {
            var dict = new QueryStringDictionary { };

            var url = GetApiUrl("System/Configuration/devices", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<DevicesOptions>(stream);
            }
        }

        public async Task<PlaybackInfoResponse> GetPlaybackInfo(PlaybackInfoRequest request)
        {
            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("UserId", request.UserId);

            var url = GetApiUrl("Items/" + request.Id + "/PlaybackInfo", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<PlaybackInfoResponse>(stream);
            }
        }

        public Task<QueryFilters> GetFilters(string userId, string parentId, string[] mediaTypes, string[] itemTypes)
        {
            throw new NotImplementedException();
        }

        public Task UpdateItem(BaseItemDto item)
        {
            throw new NotImplementedException();
        }

        public Task<SyncJob> CreateSyncJob(SyncJobRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var url = GetApiUrl("Sync/Jobs");

            return PostAsync<SyncJobRequest, SyncJob>(url, request, CancellationToken.None);
        }

        public Task UpdateSyncJob(SyncJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job");
            }

            var url = GetApiUrl("Sync/Jobs/" + job.Id);

            return PostAsync<SyncJob, EmptyRequestResult>(url, job, CancellationToken.None);
        }

        public async Task<QueryResult<SyncJobItem>> GetSyncJobItems(SyncJobItemQuery query)
        {
            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("JobId", query.JobId);
            dict.AddIfNotNull("Limit", query.Limit);
            dict.AddIfNotNull("StartIndex", query.StartIndex);
            dict.AddIfNotNullOrEmpty("TargetId", query.TargetId);
            dict.AddIfNotNull("AddMetadata", query.AddMetadata);

            if (query.Statuses.Length > 0)
            {
                dict.Add("Statuses", string.Join(",", query.Statuses.Select(i => i.ToString()).ToArray()));
            }

            var url = GetApiUrl("Sync/JobItems", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<SyncJobItem>>(stream);
            }
        }

        public async Task<QueryResult<SyncJob>> GetSyncJobs(SyncJobQuery query)
        {
            var dict = new QueryStringDictionary { };

            dict.AddIfNotNull("Limit", query.Limit);
            dict.AddIfNotNull("StartIndex", query.StartIndex);
            dict.AddIfNotNull("SyncNewContent", query.SyncNewContent);
            dict.AddIfNotNullOrEmpty("TargetId", query.TargetId);

            if (query.Statuses.Length > 0)
            {
                dict.Add("Statuses", string.Join(",", query.Statuses.Select(i => i.ToString()).ToArray()));
            }

            var url = GetApiUrl("Sync/Jobs", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<SyncJob>>(stream);
            }
        }

        public Task ReportSyncJobItemTransferred(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var url = GetApiUrl("Sync/JobItems/" + id + "/Transferred");

            return PostAsync<EmptyRequestResult>(url, new Dictionary<string, string>());
        }

        public Task<Stream> GetSyncJobItemFile(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            return GetStream(GetSyncJobItemFileUrl(id), cancellationToken);
        }

        public string GetSyncJobItemFileUrl(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            return GetApiUrl("Sync/JobItems/" + id + "/File");
        }

        public Task UpdateUserConfiguration(string userId, UserConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            var url = GetApiUrl("Users/" + userId + "/Configuration");

            return PostAsync<UserConfiguration, EmptyRequestResult>(url, configuration, CancellationToken.None);
        }

        public Task ReportOfflineActions(List<UserAction> actions)
        {
            if (actions == null || actions.Count == 0)
            {
                throw new ArgumentNullException("actions");
            }

            var url = GetApiUrl("Sync/OfflineActions");

            return PostAsync<List<UserAction>, EmptyRequestResult>(url, actions, CancellationToken.None);
        }

        public async Task<List<SyncedItem>> GetReadySyncItems(string targetId)
        {
            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("TargetId", targetId);

            var url = GetApiUrl("Sync/Items/Ready", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<List<SyncedItem>>(stream);
            }
        }

        public Task<SyncDataResponse> SyncData(SyncDataRequest request)
        {
            var url = GetApiUrl("Sync/Data");

            return PostAsync<SyncDataRequest, SyncDataResponse>(url, request, CancellationToken.None);
        }

        public Task<Stream> GetSyncJobItemAdditionalFile(string id, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dict = new QueryStringDictionary { };

            dict.AddIfNotNullOrEmpty("Name", name);

            var url = GetApiUrl("Sync/JobItems/" + id + "/AdditionalFiles", dict);

            return GetStream(url, cancellationToken);
        }

        public Task CancelSyncJob(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            var url = GetApiUrl("Sync/Jobs/" + id);

            return DeleteAsync<EmptyRequestResult>(url, CancellationToken.None);
        }

        public Task CancelSyncJobItem(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            var url = GetApiUrl("Sync/JobItems/" + id);

            return DeleteAsync<EmptyRequestResult>(url, CancellationToken.None);
        }

        public Task EnableCancelledSyncJobItem(string id)
        {
            var url = GetApiUrl("Sync/JobItems/" + id + "/Enable");

            return PostAsync<EmptyRequestResult>(url, new QueryStringDictionary(), CancellationToken.None);
        }

        public Task MarkSyncJobItemForRemoval(string id)
        {
            var url = GetApiUrl("Sync/JobItems/" + id + "/MarkForRemoval");

            return PostAsync<EmptyRequestResult>(url, new QueryStringDictionary(), CancellationToken.None);
        }

        public Task QueueFailedSyncJobItemForRetry(string id)
        {
            var url = GetApiUrl("Sync/JobItems/" + id + "/Enable");

            return PostAsync<EmptyRequestResult>(url, new QueryStringDictionary(), CancellationToken.None);
        }

        public Task UnmarkSyncJobItemForRemoval(string id)
        {
            var url = GetApiUrl("Sync/JobItems/" + id + "/UnmarkForRemoval");

            return PostAsync<EmptyRequestResult>(url, new QueryStringDictionary(), CancellationToken.None);
        }

        public async Task<UserDto> GetOfflineUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var url = GetApiUrl("Users/" + id + "/Offline");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<UserDto>(stream);
            }
        }

        public async Task<SyncDialogOptions> GetSyncOptions(SyncJobRequest jobInfo)
        {
            var dict = new QueryStringDictionary();

            dict.AddIfNotNullOrEmpty("UserId", jobInfo.UserId);
            dict.AddIfNotNullOrEmpty("ParentId", jobInfo.ParentId);
            dict.AddIfNotNullOrEmpty("TargetId", jobInfo.TargetId);

            if (jobInfo.Category.HasValue)
            {
                dict.AddIfNotNullOrEmpty("Category", jobInfo.Category.Value.ToString());
            }

            if (jobInfo.ItemIds != null)
            {
                var list = jobInfo.ItemIds.ToList();
                if (list.Count > 0)
                {
                    dict.Add("ItemIds", list);
                }
            }
            
            var url = GetApiUrl("Sync/Options", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<SyncDialogOptions>(stream);
            }
        }

        public async Task<SyncDialogOptions> GetSyncOptions(SyncJob jobInfo)
        {
            var dict = new QueryStringDictionary();

            dict.AddIfNotNullOrEmpty("UserId", jobInfo.UserId);
            dict.AddIfNotNullOrEmpty("ParentId", jobInfo.ParentId);
            dict.AddIfNotNullOrEmpty("TargetId", jobInfo.TargetId);

            if (jobInfo.Category.HasValue)
            {
                dict.AddIfNotNullOrEmpty("Category", jobInfo.Category.Value.ToString());
            }

            if (jobInfo.RequestedItemIds != null)
            {
                var list = jobInfo.RequestedItemIds.ToList();
                if (list.Count > 0)
                {
                    dict.Add("ItemIds", list);
                }
            }

            var url = GetApiUrl("Sync/Options", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<SyncDialogOptions>(stream);
            }
        }

        public async Task<List<RecommendationDto>> GetMovieRecommendations(MovieRecommendationQuery query)
        {
            var dict = new QueryStringDictionary();

            dict.AddIfNotNullOrEmpty("UserId", query.UserId);
            dict.AddIfNotNullOrEmpty("ParentId", query.ParentId);
            dict.AddIfNotNull("ItemLimit", query.ItemLimit);
            dict.AddIfNotNull("CategoryLimit", query.CategoryLimit);

            if (query.Fields != null)
            {
                dict.Add("fields", query.Fields.Select(f => f.ToString()));
            }

            var url = GetApiUrl("Movies/Recommendations", dict);

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<List<RecommendationDto>>(stream);
            }
        }

        public Task<LiveStreamResponse> OpenLiveStream(LiveStreamRequest request, CancellationToken cancellationToken)
        {
            var url = GetApiUrl("LiveStreams/Open");

            return PostAsync<LiveStreamRequest, LiveStreamResponse>(url, request, cancellationToken);
        }

        public Task CancelSyncLibraryItems(string targetId, IEnumerable<string> itemIds)
        {
            var dict = new QueryStringDictionary();

            dict.Add("ItemIds", itemIds);

            var url = GetApiUrl("Sync/" + targetId + "/Items", dict);

            return DeleteAsync<EmptyRequestResult>(url, CancellationToken.None);
        }

        public Task<int> DetectMaxBitrate(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<EndPointInfo> GetEndPointInfo(CancellationToken cancellationToken)
        {
            var url = GetApiUrl("System/Endpoint");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<EndPointInfo>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetInstantMixFromItemAsync(SimilarItemsQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var url = GetInstantMixUrl(query, "Items");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }

        public async Task<QueryResult<BaseItemDto>> GetSimilarItemsAsync(SimilarItemsQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var url = GetSimilarItemListUrl(query, "Items");

            using (var stream = await GetSerializedStreamAsync(url, cancellationToken).ConfigureAwait(false))
            {
                return DeserializeFromStream<QueryResult<BaseItemDto>>(stream);
            }
        }
    }
}
