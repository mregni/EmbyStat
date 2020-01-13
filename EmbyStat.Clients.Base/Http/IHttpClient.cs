using System.Collections.Generic;
using EmbyStat.Clients.Base.Models;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Clients.Base.Http
{
    public interface IHttpClient
    {
        void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId);
        string BaseUrl { get; set; }
        string ApiKey { get; set; }
        List<PluginInfo> GetInstalledPlugins();
        ServerInfo GetServerInfo();
        List<FileSystemEntryInfo> GetLocalDrives();
        JArray GetUsers();
        JObject GetDevices();
        bool Ping();
        MediaServerUdpBroadcast SearchServer();
        List<Movie> GetMovies(string parentId, int startIndex, int limit);
        List<Boxset> GetBoxSet(string parentId);
        List<Show> GetShows(string libraryId);
        List<Season> GetSeasons(string parentId);
        List<Episode> GetEpisodes(IEnumerable<string> parentIds, string showId);
        int GetMovieCount(string parentId);
        Person GetPersonByName(string personName);
        QueryResult<BaseItemDto> GetMediaFolders();
    }
}
