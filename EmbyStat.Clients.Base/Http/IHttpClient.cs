using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Shows;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Clients.Base.Http
{
    public interface IHttpClient
    {
        Task<int> GetMediaCount(string parentId, DateTime? lastSynced, string mediaType);

        Task<T[]> GetMedia<T>(string parentId, int startIndex, int limit, DateTime? lastSynced, string itemType);

        Task<QueryResult<BaseItemDto>> GetPeople(int startIndex, int limit);
        Task<int> GetPeopleCount();

        Task<SqlShow[]> GetShows(string parentId, int startIndex, int limit, DateTime? lastSynced);
        Task<SqlSeason[]> GetSeasons(string parentId, DateTime? lastSynced);
        Task<SqlEpisode[]> GetEpisodes(string parentId, DateTime? lastSynced);



        void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId, string userId);
        string BaseUrl { get; set; }
        string ApiKey { get; set; }
        List<PluginInfo> GetInstalledPlugins();
        ServerInfo GetServerInfo();
        List<FileSystemEntryInfo> GetLocalDrives();
        JArray GetUsers();
        JObject GetDevices();
        bool Ping();
        Task<IEnumerable<MediaServerUdpBroadcast>> SearchServer();
        SqlPerson GetPersonByName(string personName);
        QueryResult<BaseItemDto> GetMediaFolders();
        Task<IEnumerable<SqlGenre>> GetGenres();
    }
}
