using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Net;
using MediaServerUser = EmbyStat.Common.Models.Entities.Users.MediaServerUser;

namespace EmbyStat.Clients.Base.Http;

public interface IBaseHttpClient
{
    void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId, string userId);
    string BaseUrl { get; set; }
    string ApiKey { get; set; }
        
    Task<int> GetMediaCount(string parentId, DateTime? lastSynced, string mediaType);
    Task<Movie[]> GetMovies(string parentId, int startIndex, int limit, DateTime? lastSynced);

    Task<IEnumerable<Person>> GetPeople(int startIndex, int limit);
    Task<int> GetPeopleCount();
        
    Task<Show[]> GetShows(string parentId, int startIndex, int limit, DateTime? lastSynced);
    Task<Show[]> GetShows(string[] showIds, int startIndex, int limit);
    Task<Season[]> GetSeasons(string parentId);
    Task<Episode[]> GetEpisodes(string parentId);
    Task<List<BaseItemDto>> GetShowsToSync(string parentId, DateTime? lastSynced, string type);

    Task<List<PluginInfo>> GetInstalledPlugins();
    Task<MediaServerInfo> GetServerInfo();
    Task<MediaServerUser[]> GetUsers();
    Task<int> GetPlayedMediaCountForUser(string userId);
    Task<MediaServerUserView[]> GetPlayedMediaForUser(string userId, int startIndex, int limit);
    Task<IEnumerable<Device>> GetDevices();
    Task<Library[]> GetLibraries();

    Task<bool> Ping();
    Task<IEnumerable<MediaServerUdpBroadcast>> SearchServer();
        
    Task<IEnumerable<Genre>> GetGenres();
}