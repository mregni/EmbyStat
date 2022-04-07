using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Common.Models.Net;

namespace EmbyStat.Clients.Base.Http
{
    public interface IBaseHttpClient
    {
        void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId, string userId);
        string BaseUrl { get; set; }
        string ApiKey { get; set; }
        
        Task<int> GetMediaCount(string parentId, DateTime? lastSynced, string mediaType);
        Task<T[]> GetMedia<T>(string parentId, int startIndex, int limit, DateTime? lastSynced, string itemType);

        Task<IEnumerable<Person>> GetPeople(int startIndex, int limit);
        Task<int> GetPeopleCount();
        
        Task<Show[]> GetShows(string parentId, int startIndex, int limit, DateTime? lastSynced);
        Task<Season[]> GetSeasons(string parentId, DateTime? lastSynced);
        Task<Episode[]> GetEpisodes(string parentId, DateTime? lastSynced);

        Task<List<PluginInfo>> GetInstalledPlugins();
        Task<MediaServerInfo> GetServerInfo();
        Task<List<MediaServerUser>> GetUsers();
        Task<IEnumerable<Device>> GetDevices();
        Task<Library[]> GetLibraries();

        Task<bool> Ping();
        Task<IEnumerable<MediaServerUdpBroadcast>> SearchServer();
        
        Task<IEnumerable<Genre>> GetGenres();
    }
}
