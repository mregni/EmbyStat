using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EmbyStat.Clients.Base.Converters;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Logging;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace EmbyStat.Clients.Base.Http
{
    public abstract class BaseHttpClient
    {
        protected readonly Logger Logger;
        protected string DeviceName { get; set; }
        protected string ApplicationVersion { get; set; }
        protected string DeviceId { get; set; }
        protected string UserId { get; set; }

        protected string apiKey { get; set; }
        public string ApiKey
        {
            get => apiKey;
            set => apiKey = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }

        public string BaseUrl
        {
            get => RestClient.BaseUrl?.ToString() ?? string.Empty;
            set => RestClient.BaseUrl = string.IsNullOrWhiteSpace(value) ? null : new Uri(value);
        }

        protected string AuthorizationScheme { get; set; }
        protected string AuthorizationParameter => $"RestClient=\"other\", DeviceId=\"{DeviceId}\", Device=\"{DeviceName}\", Version=\"{ApplicationVersion}\"";

        protected readonly IRestClient RestClient;

        protected BaseHttpClient(IRestClient client)
        {
            Logger = LogFactory.CreateLoggerForType(typeof(BaseHttpClient), "BASE-HTTP-CLIENT");
            RestClient = client.Initialize();
        }

        public void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId, string userId)
        {
            AuthorizationScheme = authorizationScheme;
            ApplicationVersion = applicationVersion;
            DeviceId = deviceId;
            DeviceName = deviceName;
            UserId = userId;
        }

        protected string ExecuteCall(IRestRequest request)
        {
            request.AddHeader("X-Emby-Authorization", $"{AuthorizationScheme} {AuthorizationParameter}");

            Logger.Debug($"External call: [{request.Method}]{RestClient.BaseUrl}/{request.Resource}");
            var result = RestClient.Execute(request);

            if (!result.IsSuccessful)
            {
                Logger.Debug($"Call failed => StatusCode:{result.StatusCode}, Content:{result.Content}");
            }

            return result.Content;
        }

        protected T ExecuteCall<T>(IRestRequest request) where T : new()
        {
            request.AddHeader("X-Emby-Authorization", $"{AuthorizationScheme} {AuthorizationParameter}");

            Logger.Debug($"External call: [{request.Method}]{RestClient.BaseUrl}{request.Resource}");
            var result = RestClient.Execute<T>(request);

            if (!result.IsSuccessful)
            {
                Logger.Debug($"Call failed => StatusCode:{result.StatusCode}, Content:{result.Content}");
            }

            if (result.Data == null)
            {
                Logger.Debug($"Returned object cant be parsed to {typeof(T).Name}");
                Logger.Debug($"RAW content: {result.Content}");
            }

            return result.Data;
        }

        protected T ExecuteAuthenticatedCall<T>(IRestRequest request) where T : new()
        {
            request.AddHeader("X-Emby-Token", ApiKey);
            return ExecuteCall<T>(request);
        }

        protected MediaServerUdpBroadcast SearchServer(string message)
        {
            using var client = new UdpClient();
            var requestData = Encoding.ASCII.GetBytes(message);
            var serverEp = new IPEndPoint(IPAddress.Any, 7359);

            client.EnableBroadcast = true;
            client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, 7359));

            var timeToWait = TimeSpan.FromSeconds(3);

            var asyncResult = client.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(timeToWait);
            if (asyncResult.IsCompleted)
            {
                try
                {
                    var receivedData = client.EndReceive(asyncResult, ref serverEp);
                    var serverResponse = Encoding.ASCII.GetString(receivedData);
                    var udpBroadcastResult = JsonConvert.DeserializeObject<MediaServerUdpBroadcast>(serverResponse);
                    return udpBroadcastResult;
                }
                catch (Exception)
                {
                    // No data received, swallow exception and return empty object
                }
            }

            return null;
        }

        public bool Ping(string message)
        {
            var request = new RestRequest("System/Ping", Method.POST) { Timeout = 5000 };

            try
            {
                var result = ExecuteCall(request);
                Logger.Debug($"Ping returned: {result}");
                return result == message;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Ping failed");
                return false;
            }
        }

        public ServerInfo GetServerInfo()
        {
            var request = new RestRequest("System/Info", Method.GET);
            var result = ExecuteAuthenticatedCall<ServerInfoDto>(request);

            return result.ConvertToInfo();
        }

        public Person GetPersonByName(string personName)
        {
            var request = new RestRequest($"persons/{personName}", Method.GET);
            request.AddItemQueryAsParameters(new ItemQuery { Fields = new[] { ItemFields.PremiereDate } }, UserId);
            var baseItem = ExecuteAuthenticatedCall<BaseItemDto>(request);
            return baseItem != null ? PersonConverter.Convert(baseItem, Logger) : null;
        }

        public QueryResult<BaseItemDto> GetMediaFolders()
        {
            var request = new RestRequest("Library/MediaFolders", Method.GET);
            return ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
        }

        public List<PluginInfo> GetInstalledPlugins()
        {
            var request = new RestRequest("Plugins", Method.GET);
            return ExecuteAuthenticatedCall<List<PluginInfo>>(request);
        }

        public List<FileSystemEntryInfo> GetLocalDrives()
        {
            var request = new RestRequest("Environment/Drives", Method.GET);
            return ExecuteAuthenticatedCall<List<FileSystemEntryInfo>>(request);
        }

        public JArray GetUsers()
        {
            var request = new RestRequest("Users", Method.GET);
            return ExecuteAuthenticatedCall<JArray>(request);
        }

        public JObject GetDevices()
        {
            var request = new RestRequest("Devices", Method.GET);
            return ExecuteAuthenticatedCall<JObject>(request);
        }

        public List<Movie> GetMovies(string parentId, string collectionId, int startIndex, int limit)
        {
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                LocationTypes = new[] { LocationType.FileSystem },
                IncludeItemTypes = new[] { nameof(Movie) },
                StartIndex = startIndex,
                Limit = limit,
                EnableImages = true,
                Fields = new[] {
                    ItemFields.Genres, ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.ExternalUrls,
                    ItemFields.OriginalTitle, ItemFields.Studios, ItemFields.MediaStreams, ItemFields.Path,
                    ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId,
                    ItemFields.People, ItemFields.PremiereDate, ItemFields.CommunityRating, ItemFields.OfficialRating,
                    ItemFields.ProductionYear, ItemFields.RunTimeTicks
                }
            };

            var request = new RestRequest($"Items", Method.GET);
            request.AddItemQueryAsParameters(query, UserId);
            var baseItems = ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
            return baseItems?.Items != null 
                ? baseItems.Items.Select(x => x.ConvertToMovie(collectionId, Logger)).ToList() 
                : new List<Movie>(0);
        }

        public List<BoxSet> GetBoxSet(string parentId)
        {
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Primary },
                ParentId = parentId,
                Recursive = true,
                LocationTypes = new[] { LocationType.FileSystem },
                IncludeItemTypes = new[] { nameof(BoxSet) },
                Fields = new[]
                {
                    ItemFields.Genres, ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.ExternalUrls,
                    ItemFields.OriginalTitle, ItemFields.Studios, ItemFields.MediaStreams, ItemFields.Path,
                    ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId,
                    ItemFields.People, ItemFields.PremiereDate, ItemFields.CommunityRating, ItemFields.OfficialRating,
                    ItemFields.ProductionYear, ItemFields.RunTimeTicks
                }
            };

            var request = new RestRequest($"Items", Method.GET);
            request.AddItemQueryAsParameters(query, UserId);
            var baseItems = ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
            return baseItems?.Items != null
                ? baseItems.Items.Select(x => x.ConvertToBoxSet(Logger)).ToList()
                : new List<BoxSet>(0);
        }

        public List<Show> GetShows(string parentId)
        {
            var query = new ItemQuery
            {
                SortBy = new[] { "SortName" },
                SortOrder = SortOrder.Ascending,
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                LocationTypes = new[] { LocationType.FileSystem },
                Recursive = true,
                IncludeItemTypes = new[] { "Series" },
                Fields = new[] {
                    ItemFields.OriginalTitle, ItemFields.Genres, ItemFields.DateCreated, ItemFields.ExternalUrls,
                    ItemFields.Studios, ItemFields.Path, ItemFields.ProviderIds,
                    ItemFields.SortName, ItemFields.ParentId, ItemFields.People, ItemFields.PremiereDate,
                    ItemFields.CommunityRating, ItemFields.OfficialRating, ItemFields.ProductionYear,
                    ItemFields.Status, ItemFields.RunTimeTicks
                }
            };

            var request = new RestRequest($"Items", Method.GET);
            request.AddItemQueryAsParameters(query, UserId);
            var baseItems = ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
            return baseItems?.Items != null
                ? baseItems.Items.Select(x => x.ConvertToShow(parentId, Logger)).ToList()
                : new List<Show>(0);
        }

        public List<Season> GetSeasons(string parentId)
        {
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                LocationTypes = new[] { LocationType.FileSystem },
                Recursive = true,
                IncludeItemTypes = new[] { nameof(Season) },
                Fields = new[]
                {
                    ItemFields.OriginalTitle,ItemFields.Genres, ItemFields.DateCreated, ItemFields.ExternalUrls,
                    ItemFields.Studios, ItemFields.Path, ItemFields.Overview, ItemFields.ProviderIds,
                    ItemFields.SortName, ItemFields.ParentId, ItemFields.People, ItemFields.MediaSources,
                    ItemFields.MediaStreams, ItemFields.PremiereDate, ItemFields.CommunityRating,
                    ItemFields.OfficialRating, ItemFields.ProductionYear, ItemFields.Status, ItemFields.RunTimeTicks
                }
            };

            var request = new RestRequest($"Items", Method.GET);
            request.AddItemQueryAsParameters(query, UserId);
            var baseItems = ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
            return baseItems?.Items != null
                ? baseItems.Items.Select(x => x.ConvertToSeason(Logger)).ToList()
                : new List<Season>(0);
        }

        public List<Episode> GetEpisodes(IEnumerable<string> parentIds, string showId)
        {
            var episodes = new List<Episode>();
            foreach (var parentId in parentIds)
            {
                var query = new ItemQuery
                {
                    EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                    ParentId = parentId,
                    LocationTypes = new[] { LocationType.FileSystem },
                    Recursive = true,
                    IncludeItemTypes = new[] { nameof(Episode) },
                    Fields = new[]
                    {
                        ItemFields.OriginalTitle,ItemFields.Genres, ItemFields.DateCreated, ItemFields.ExternalUrls,
                        ItemFields.Studios, ItemFields.Path, ItemFields.Overview, ItemFields.ProviderIds,
                        ItemFields.SortName, ItemFields.ParentId, ItemFields.People, ItemFields.MediaSources,
                        ItemFields.MediaStreams, ItemFields.PremiereDate, ItemFields.CommunityRating,
                        ItemFields.OfficialRating, ItemFields.ProductionYear, ItemFields.Status, ItemFields.RunTimeTicks
                    }
                };

                var request = new RestRequest($"Items", Method.GET);
                request.AddItemQueryAsParameters(query, UserId);
                var baseItems = ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
                if (baseItems?.Items != null)
                {
                    episodes
                        .AddRange(baseItems.Items.Select(x => x.ConvertToEpisode(showId, Logger))
                        .Where(x => x != null));
                }
            }

            return episodes;
        }

        public int GetMovieCount(string parentId)
        {
            var query = new ItemQuery
            {
                ParentId = parentId,
                Recursive = true,
                IncludeItemTypes = new[] { nameof(Movie), nameof(BoxSet) },
                StartIndex = 0,
                Limit = 1,
                EnableTotalRecordCount = true
            };

            var request = new RestRequest($"Items", Method.GET);
            request.AddItemQueryAsParameters(query, UserId);
            var baseItems = ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
            return baseItems?.TotalRecordCount ?? 0;
        }
    }
}
