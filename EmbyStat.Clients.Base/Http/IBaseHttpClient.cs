using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Common.SqLite.Users;
using MediaBrowser.Model.IO;

namespace EmbyStat.Clients.Base.Http
{
    public interface IBaseHttpClient
    {
        void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId, string userId);
        string BaseUrl { get; set; }
        string ApiKey { get; set; }
        
        Task<int> GetMediaCount(string parentId, DateTime? lastSynced, string mediaType);
        Task<T[]> GetMedia<T>(string parentId, int startIndex, int limit, DateTime? lastSynced, string itemType);

        Task<IEnumerable<SqlPerson>> GetPeople(int startIndex, int limit);
        Task<int> GetPeopleCount();
        SqlPerson GetPersonByName(string personName);
        
        Task<SqlShow[]> GetShows(string parentId, int startIndex, int limit, DateTime? lastSynced);
        Task<SqlSeason[]> GetSeasons(string parentId, DateTime? lastSynced);
        Task<SqlEpisode[]> GetEpisodes(string parentId, DateTime? lastSynced);

        Task<List<SqlPluginInfo>> GetInstalledPlugins();
        Task<ServerInfoDto> GetServerInfo();
        List<FileSystemEntryInfo> GetLocalDrives();
        Task<List<SqlUser>> GetUsers();
        Task<IEnumerable<SqlDevice>> GetDevices();
        Task<Library[]> GetLibraries();

        bool Ping();
        Task<IEnumerable<MediaServerUdpBroadcast>> SearchServer();
        
        Task<IEnumerable<SqlGenre>> GetGenres();
    }
}
