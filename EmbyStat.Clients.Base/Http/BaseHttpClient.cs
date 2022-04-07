using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base.Api;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Common.Models.Net;
using EmbyStat.Logging;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Http;

namespace EmbyStat.Clients.Base.Http
{
    public abstract class BaseHttpClient
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IRefitHttpClientFactory<IMediaServerApi> _refitFactory;
        private readonly Logger _logger;
        private readonly IMapper _mapper;

        private string DeviceName { get; set; }
        private string ApplicationVersion { get; set; }
        private string DeviceId { get; set; }
        private string UserId { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        private string AuthorizationScheme { get; set; }
        private string AuthorizationParameter => $"RestClient=\"other\", DeviceId=\"{DeviceId}\", Device=\"{DeviceName}\", Version=\"{ApplicationVersion}\"";
        private string AuthorizationString => $"{AuthorizationScheme} {AuthorizationParameter}";

        protected BaseHttpClient(IHttpContextAccessor accessor, 
            IRefitHttpClientFactory<IMediaServerApi> refitFactory, IMapper mapper)
        {
            _accessor = accessor;
            _refitFactory = refitFactory;
            _mapper = mapper;
            _logger = LogFactory.CreateLoggerForType(typeof(BaseHttpClient), "BASE-HTTP-CLIENT");
        }

        public void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId, string userId)
        {
            AuthorizationScheme = authorizationScheme;
            ApplicationVersion = applicationVersion;
            DeviceId = deviceId;
            DeviceName = deviceName;
            UserId = userId;
        }

        protected async Task<IEnumerable<MediaServerUdpBroadcast>> SearchServer(string message)
        {
            var list = new List<MediaServerUdpBroadcast>();
            var ownIp = _accessor.HttpContext?.Connection.RemoteIpAddress ?? IPAddress.Any;
            _logger.Debug($"Own IP detected: {ownIp.MapToIPv4()}");
            _logger.Debug($"Sending \"{message}\" to following broadcast IPs:");
            foreach (var ip in GetBroadCastIps(ownIp))
            {
                _logger.Debug($"\t{ip.MapToIPv4()}");
                await Task.Run(async () =>
                {
                    var to = new IPEndPoint(ip, 7359);
                    using var client = new ServerSearcher(to);

                    client.MediaServerFound += (_, broadcast) =>
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
            _logger.Debug($"{ip.MapToIPv4()})");
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in interfaces)
            {
                foreach (var uniCastInfo in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (uniCastInfo.Address.AddressFamily != AddressFamily.InterNetwork
                    || uniCastInfo.Address.MapToIPv4().Equals(IPAddress.Parse("127.0.0.1")))
                    {
                        continue;
                    }

                    yield return GetBroadcastAddress(uniCastInfo);
                }
            }
        }

        private IPAddress GetBroadcastAddress(UnicastIPAddressInformation uniCastAddress)
        {
            return GetBroadcastAddress(uniCastAddress.Address, uniCastAddress.IPv4Mask);
        }

        private IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
        {
            var ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
            var ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            var broadCastIpAddress = ipAddress | ~ipMaskV4;

            return new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
        }

        protected async Task<bool> Ping(string message)
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.Ping(ApiKey, AuthorizationString);
            _logger.Debug($"Ping returned: {result}");
            return result == message;
        }

        public async Task<MediaServerInfo> GetServerInfo()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            var response = await client.GetServerInfo(ApiKey, AuthorizationString);

            return response.IsSuccessStatusCode ? response.Content : null;
        }

        public async Task<IEnumerable<Person>> GetPeople(int startIndex, int limit)
        {
            var query = new ItemQuery
            {
                Recursive = true,
                StartIndex = startIndex,
                Limit = limit
            };

            var client = _refitFactory.CreateClient(BaseUrl);
            var people = await client.GetPeople(ApiKey, AuthorizationString, query);
            return _mapper.Map<IList<Person>>(people.Items);
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
            var result = await client.GetPeople(ApiKey, AuthorizationString, query);
            return result.TotalRecordCount;
        }

        public async Task<Library[]> GetLibraries()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetMediaFolders(ApiKey, AuthorizationString);
            return _mapper.Map<Library[]>(result.Items);
        }

        public Task<List<PluginInfo>> GetInstalledPlugins()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            return client.GetPlugins(ApiKey, AuthorizationString);
        }

        public Task<List<MediaServerUser>> GetUsers()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            return client.GetUsers(ApiKey, AuthorizationString);
        }

        public async Task<IEnumerable<Device>> GetDevices()
        {
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetDevices(ApiKey, AuthorizationString);
            return result.Items;
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            var query = new ItemQuery();
            var client = _refitFactory.CreateClient(BaseUrl);
            var baseItems = await client.GetGenres(ApiKey, AuthorizationString, query);
            return _mapper.Map<IList<Genre>>(baseItems.Items);
        }

        public async Task<T[]> GetMedia<T>(string parentId, int startIndex, int limit, DateTime? lastSynced, string itemType)
        {
            var query = new ItemQuery
            {
                UserId = UserId,
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

            var paramList = query.ConvertToStringDictionary();
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(ApiKey, AuthorizationString, paramList);

            return _mapper.Map<T[]>(result.Items);
        }

        public async Task<Show[]> GetShows(string parentId, int startIndex, int limit, DateTime? lastSynced)
        {
            var query = new ItemQuery
            {
                UserId = UserId,
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

            var paramList = query.ConvertToStringDictionary();
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(ApiKey, AuthorizationString, paramList);

            var shows = _mapper.Map<Show[]>(result.Items);
            return shows;
        }

        public async Task<Season[]> GetSeasons(string parentId, DateTime? lastSynced)
        {
            var query = new ItemQuery
            {
                UserId = UserId,
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

            var paramList = query.ConvertToStringDictionary();
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(ApiKey, AuthorizationString, paramList);

            var seasons = _mapper.Map<Season[]>(result.Items);
            return seasons;
        }

        public async Task<Episode[]> GetEpisodes(string parentId, DateTime? lastSynced)
        {
            var query = new ItemQuery
            {
                UserId = UserId,
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

            var paramList = query.ConvertToStringDictionary();
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(ApiKey, AuthorizationString, paramList);

            var episodes = _mapper.Map<Episode[]>(result.Items);
            return episodes;
        }

        public async Task<int> GetMediaCount(string parentId, DateTime? lastSynced, string mediaType)
        {
            var query = new ItemQuery
            {
                UserId = UserId,
                ParentId = parentId,
                Recursive = true,
                IncludeItemTypes = new[] { mediaType },
                ExcludeLocationTypes = new[] { LocationType.Virtual },
                Limit = 0,
                EnableTotalRecordCount = true,
                MinDateLastSaved = lastSynced
            };

            var paramList = query.ConvertToStringDictionary();
            var client = _refitFactory.CreateClient(BaseUrl);
            var result = await client.GetItems(ApiKey, AuthorizationString, paramList);
            return result.TotalRecordCount;
        }
    }
}
