﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Shows;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Clients.Base.Http
{
    public interface IHttpClient
    {
        Task<int> GetMovieCount(string parentId, DateTime? lastSynced);
        Task<SqlMovie[]> GetMovies(string parentId, int startIndex, int limit, DateTime? lastSynced);

        Task<QueryResult<BaseItemDto>> GetPeople(int startIndex, int limit);
        Task<int> GetPeopleCount();

        Task<QueryResult<SqlShow>> GetShows(string parentId, int startIndex, int limit, DateTime? lastSynced);
        List<Season> GetSeasons(string parentId, DateTime? lastSynced);
        List<Episode> GetEpisodes(string parentId, string showId, DateTime? lastSynced);

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
