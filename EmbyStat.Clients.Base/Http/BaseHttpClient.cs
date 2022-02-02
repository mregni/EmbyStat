using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base.Converters;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Logging;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace EmbyStat.Clients.Base.Http
{
    public abstract class BaseHttpClient
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IRefitHttpClientFactory<INewBaseClient> _refitClient;
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
            get => RestClient?.BaseUrl?.ToString() ?? string.Empty;
            set => RestClient.BaseUrl = string.IsNullOrWhiteSpace(value) ? null : new Uri(value);
        }

        protected string AuthorizationScheme { get; set; }
        protected string AuthorizationParameter => $"RestClient=\"other\", DeviceId=\"{DeviceId}\", Device=\"{DeviceName}\", Version=\"{ApplicationVersion}\"";
        private string AuthorizationString => "{AuthorizationScheme} {AuthorizationParameter}";

        protected readonly IRestClient RestClient;
        protected readonly IMapper Mapper;

        protected BaseHttpClient(IRestClient client, IHttpContextAccessor accessor, 
            IRefitHttpClientFactory<INewBaseClient> refitClient, IMapper mapper)
        {
            _accessor = accessor;
            _refitClient = refitClient;
            Mapper = mapper;
            RestClient = client.Initialize();
            Logger = LogFactory.CreateLoggerForType(typeof(BaseHttpClient), "BASE-HTTP-CLIENT");
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


        protected async Task<IEnumerable<MediaServerUdpBroadcast>> SearchServer(string message)
        {
            var list = new List<MediaServerUdpBroadcast>();
            var ownIp = _accessor.HttpContext?.Connection.RemoteIpAddress ?? IPAddress.Any;
            Logger.Debug($"Own IP detected: {ownIp.MapToIPv4()}");
            Logger.Debug($"Sending \"{message}\" to following broadcast IPs:");
            foreach (var ip in GetBroadCastIps(ownIp))
            {
                Logger.Debug($"\t{ip.MapToIPv4()}");
                await Task.Run(async () =>
                {
                    var to = new IPEndPoint(ip, 7359);
                    using var client = new ServerSearcher(to);

                    client.MediaServerFound += (sender, broadcast) =>
                    {
                        list.Add(broadcast);
                    };

                    client.Send(message);
                    await Task.Run(() => Task.Delay(3000));
                });
            }

            return list;
        }

        private IEnumerable<IPAddress> GetBroadCastIps(IPAddress ip)
        {
            Logger.Debug($"{ip.MapToIPv4()})");
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in interfaces)
            {
                foreach (var unicastInfo in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastInfo.Address.AddressFamily != AddressFamily.InterNetwork
                    || unicastInfo.Address.MapToIPv4().Equals(IPAddress.Parse("127.0.0.1")))
                    {
                        continue;
                    }

                    if (CheckWhetherInSameNetwork(unicastInfo.Address, unicastInfo.IPv4Mask, ip))
                        yield return GetBroadcastAddress(unicastInfo);
                }
            }
        }

        private bool CheckWhetherInSameNetwork(IPAddress firstIp, IPAddress subNet, IPAddress secondIp)
        {
            var subnetmaskInInt = ConvertIpToUint(subNet);
            var firstIpInInt = ConvertIpToUint(firstIp);
            var secondIpInInt = ConvertIpToUint(secondIp);
            var networkPortionofFirstIp = firstIpInInt & subnetmaskInInt;
            var networkPortionofSecondIp = secondIpInInt & subnetmaskInInt;
            return networkPortionofFirstIp == networkPortionofSecondIp;
        }

        private static uint ConvertIpToUint(IPAddress ipAddress)
        {
            var byteIp = ipAddress.GetAddressBytes();
            var ipInUint = (uint)byteIp[3] << 24;
            ipInUint += (uint)byteIp[2] << 16;
            ipInUint += (uint)byteIp[1] << 8;
            ipInUint += byteIp[0];
            return ipInUint;
        }

        private IPAddress GetBroadcastAddress(UnicastIPAddressInformation unicastAddress)
        {
            return GetBroadcastAddress(unicastAddress.Address, unicastAddress.IPv4Mask);
        }

        private IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
        {
            var ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
            var ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            var broadCastIpAddress = ipAddress | ~ipMaskV4;

            return new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
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
            catch (System.Exception e)
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

        public SqlPerson GetPersonByName(string personName)
        {
            var request = new RestRequest($"persons/{personName}", Method.GET);
            //request.AddItemQueryAsParameters(new ItemQuery { Fields = new[] { ItemFields.PremiereDate } }, UserId);
            var baseItem = ExecuteAuthenticatedCall<BaseItemDto>(request);
            return baseItem?.ConvertToPeople(Logger);
        }

        public async Task<QueryResult<BaseItemDto>> GetPeople(int startIndex, int limit)
        {
            var query = new ItemQuery
            {
                StartIndex = startIndex,
                Limit = limit,
                Fields = new[] { ItemFields.PremiereDate }
            };

            var client = _refitClient.CreateClient(BaseUrl);
            return await client.GetPeople(apiKey, AuthorizationString, query);
        }

        public async Task<int> GetPeopleCount()
        {
            var query = new ItemQuery
            {
                Recursive = true,
                Limit = 0,
                EnableTotalRecordCount = true,
            };

            var paramList = query.ConvertToStringDictionary(UserId);
            var client = _refitClient.CreateClient(BaseUrl);
            var result = await client.GetPeople(apiKey, AuthorizationString, query);
            return result.TotalRecordCount;
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

        public async Task<IEnumerable<SqlGenre>> GetGenres()
        {
            var query = new ItemQuery();
            var client = _refitClient.CreateClient(BaseUrl);
            var baseItems = await client.GetGenres(apiKey, AuthorizationString, query);
            return baseItems?.Items != null
                ? baseItems.Items.Select(x => x.ConvertToGenre(Logger)).ToList()
                : new List<SqlGenre>(0);
        }

        public async Task<QueryResult<SqlMovie>> GetMovies(string parentId, int startIndex, int limit, DateTime? lastSynced)
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
                MinDateLastSaved = lastSynced,
                Fields = new[] {
                    ItemFields.Genres, ItemFields.DateCreated, ItemFields.MediaSources, ItemFields.ExternalUrls,
                    ItemFields.OriginalTitle, ItemFields.Studios, ItemFields.MediaStreams, ItemFields.Path,
                    ItemFields.Overview, ItemFields.ProviderIds, ItemFields.SortName, ItemFields.ParentId,
                    ItemFields.People, ItemFields.PremiereDate, ItemFields.CommunityRating, ItemFields.OfficialRating,
                    ItemFields.ProductionYear, ItemFields.RunTimeTicks
                }
            };

            var paramList = query.ConvertToStringDictionary(UserId);
            var client = _refitClient.CreateClient(BaseUrl);
            var result = await client.GetItems(apiKey, AuthorizationString, paramList);

            var movieQueryResult = Mapper.Map<QueryResult<SqlMovie>>(result);
            return movieQueryResult;
        }

        public async Task<QueryResult<SqlShow>> GetShows(string parentId, int startIndex, int limit, DateTime? lastSynced)
        {
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                LocationTypes = new[] { LocationType.FileSystem },
                Recursive = true,
                IncludeItemTypes = new[] { "Series" },
                MinDateLastSaved = lastSynced,
                Fields = new[] {
                    ItemFields.OriginalTitle, ItemFields.Genres, ItemFields.DateCreated, ItemFields.ExternalUrls,
                    ItemFields.Studios, ItemFields.Path, ItemFields.ProviderIds,
                    ItemFields.SortName, ItemFields.ParentId, ItemFields.People, ItemFields.PremiereDate,
                    ItemFields.CommunityRating, ItemFields.OfficialRating, ItemFields.ProductionYear,
                    ItemFields.Status, ItemFields.RunTimeTicks
                }
            };

            var paramList = query.ConvertToStringDictionary(UserId);
            var client = _refitClient.CreateClient(BaseUrl);
            var result = await client.GetItems(apiKey, AuthorizationString, paramList);

            var showQueryResult = Mapper.Map<QueryResult<SqlShow>>(result);
            return showQueryResult;
        }

        public List<Season> GetSeasons(string parentId, DateTime? lastSynced)
        {
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                LocationTypes = new[] { LocationType.FileSystem },
                Recursive = true,
                IncludeItemTypes = new[] { nameof(Season) },
                MinDateLastSaved = lastSynced,
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
            //request.AddItemQueryAsParameters(query, UserId);
            var baseItems = ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
            return baseItems?.Items != null
                ? baseItems.Items.Select(x => x.ConvertToSeason(Logger)).ToList()
                : new List<Season>(0);
        }

        public List<Episode> GetEpisodes(string parentId, string showId, DateTime? lastSynced)
        {
            var episodes = new List<Episode>();
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                LocationTypes = new[] { LocationType.FileSystem },
                Recursive = true,
                IncludeItemTypes = new[] { nameof(Episode) },
                MinDateLastSaved = lastSynced,
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
            //request.AddItemQueryAsParameters(query, UserId);
            var baseItems = ExecuteAuthenticatedCall<QueryResult<BaseItemDto>>(request);
            if (baseItems?.Items != null)
            {
                episodes
                    .AddRange(baseItems.Items.Select(x => x.ConvertToEpisode(showId, Logger))
                    .Where(x => x != null));
            }

            return episodes;
        }

        public async Task<int> GetMovieCount(string parentId, DateTime? lastSynced)
        {
            var query = new ItemQuery
            {
                ParentId = parentId,
                Recursive = true,
                IncludeItemTypes = new[] { nameof(Movie) },
                ExcludeLocationTypes = new[] { LocationType.Virtual },
                Limit = 0,
                EnableTotalRecordCount = true,
                MinDateLastSaved = lastSynced
            };

            var paramList = query.ConvertToStringDictionary(UserId);
            var client = _refitClient.CreateClient(BaseUrl);
            var result = await client.GetItems(apiKey, AuthorizationString, paramList);
            return result.TotalRecordCount;
        }
    }
}
