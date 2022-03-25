using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base.Converters;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Common.SqLite.Users;
using EmbyStat.Logging;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace EmbyStat.Clients.Base.Http
{
    public abstract class BaseHttpClient
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IRefitHttpClientFactory<INewBaseClient> _refitFactory;
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
            IRefitHttpClientFactory<INewBaseClient> refitFactory, IMapper mapper)
        {
            _accessor = accessor;
            _refitFactory = refitFactory;
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

        public async Task<ServerInfoDto> GetServerInfo()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            return await client.GetServerInfo(apiKey, AuthorizationString);
        }

        public SqlPerson GetPersonByName(string personName)
        {
            var request = new RestRequest($"persons/{personName}", Method.GET);
            var baseItem = ExecuteAuthenticatedCall<BaseItemDto>(request);
            return Mapper.Map<SqlPerson>(baseItem);
        }

        public async Task<IEnumerable<SqlPerson>> GetPeople(int startIndex, int limit)
        {
            var query = new ItemQuery
            {
                StartIndex = startIndex,
                Limit = limit,
                Fields = new[] { ItemFields.PremiereDate }
            };

            var client = _refitFactory.CreateClient(BaseUrl);
            var people = await client.GetPeople(apiKey, AuthorizationString, query);
            return Mapper.Map<IList<SqlPerson>>(people.Items);
        }

        public async Task<int> GetPeopleCount()
        {
            var query = new ItemQuery
            {
                Recursive = true,
                Limit = 0,
                EnableTotalRecordCount = true,
            };

            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetPeople(apiKey, AuthorizationString, query);
            return result.TotalRecordCount;
        }

        public async Task<Library[]> GetLibraries()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetMediaFolders(apiKey, AuthorizationString);
            return Mapper.Map<Library[]>(result.Items);
        }

        public Task<List<SqlPluginInfo>> GetInstalledPlugins()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            return client.GetPlugins(apiKey, AuthorizationString);
        }

        public List<FileSystemEntryInfo> GetLocalDrives()
        {
            var request = new RestRequest("Environment/Drives", Method.GET);
            return ExecuteAuthenticatedCall<List<FileSystemEntryInfo>>(request);
        }

        public Task<List<SqlUser>> GetUsers()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            return client.GetUsers(apiKey, AuthorizationString);
        }

        public async Task<IEnumerable<SqlDevice>> GetDevices()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            var response = await client.GetDevices(apiKey, AuthorizationString);
            return response.Content?.Items;
        }

        public async Task<IEnumerable<SqlGenre>> GetGenres()
        {
            var query = new ItemQuery();
            var client = _refitFactory.CreateClient(BaseUrl);
            var baseItems = await client.GetGenres(apiKey, AuthorizationString, query);
            return baseItems?.Items != null
                ? baseItems.Items.Select(x => x.ConvertToGenre(Logger)).ToList()
                : new List<SqlGenre>(0);
        }

        public async Task<T[]> GetMedia<T>(string parentId, int startIndex, int limit, DateTime? lastSynced, string itemType)
        {
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                Recursive = true,
                LocationTypes = new[] { LocationType.FileSystem },
                IncludeItemTypes = new[] { itemType },
                StartIndex = startIndex,
                Limit = limit,
                EnableImages = true,
                MediaTypes = new []{"Video"},
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
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(apiKey, AuthorizationString, paramList);

            return Mapper.Map<T[]>(result.Items);
        }

        public async Task<SqlShow[]> GetShows(string parentId, int startIndex, int limit, DateTime? lastSynced)
        {
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Banner, ImageType.Primary, ImageType.Thumb, ImageType.Logo },
                ParentId = parentId,
                LocationTypes = new[] { LocationType.FileSystem },
                Recursive = true,
                StartIndex = startIndex,
                Limit = limit,
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
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(apiKey, AuthorizationString, paramList);

            var shows = Mapper.Map<SqlShow[]>(result.Items);
            return shows;
        }

        public async Task<SqlSeason[]> GetSeasons(string parentId, DateTime? lastSynced)
        {
            var query = new ItemQuery
            {
                EnableImageTypes = new[] { ImageType.Primary },
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

            var paramList = query.ConvertToStringDictionary(UserId);
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(apiKey, AuthorizationString, paramList);

            var seasons = Mapper.Map<SqlSeason[]>(result.Items);
            return seasons;
        }

        public async Task<SqlEpisode[]> GetEpisodes(string parentId, DateTime? lastSynced)
        {
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
                        ItemFields.SortName, ItemFields.ParentId, ItemFields.MediaSources,
                        ItemFields.MediaStreams, ItemFields.PremiereDate, ItemFields.CommunityRating,
                        ItemFields.OfficialRating, ItemFields.ProductionYear, ItemFields.Status, ItemFields.RunTimeTicks
                    }
            };

            var paramList = query.ConvertToStringDictionary(UserId);
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(apiKey, AuthorizationString, paramList);

            var episodes = Mapper.Map<SqlEpisode[]>(result.Items);
            return episodes;
        }

        public async Task<int> GetMediaCount(string parentId, DateTime? lastSynced, string mediaType)
        {
            var query = new ItemQuery
            {
                ParentId = parentId,
                Recursive = true,
                IncludeItemTypes = new[] { mediaType },
                ExcludeLocationTypes = new[] { LocationType.Virtual },
                Limit = 0,
                EnableTotalRecordCount = true,
                MinDateLastSaved = lastSynced
            };

            var paramList = query.ConvertToStringDictionary(UserId);
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(apiKey, AuthorizationString, paramList);
            return result.TotalRecordCount;
        }
    }
}
