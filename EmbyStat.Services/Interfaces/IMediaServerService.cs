using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IMediaServerService
    {
        #region Server

        MediaServerUdpBroadcast SearchMediaServer(ServerType type);
        ServerInfo GetServerInfo(bool forceReSync);
        bool TestNewApiKey(string url, string apiKey, ServerType type);
        EmbyStatus GetMediaServerStatus();
        bool PingMediaServer(string url);
        void ResetMissedPings();
        void IncreaseMissedPings();
        void ResetMediaServerData();
        IEnumerable<Library> GetMediaServerLibraries();

        #endregion

        #region Plugins

        List<PluginInfo> GetAllPlugins();

        #endregion

        #region Users

        List<EmbyUser> GetAllUsers();
        List<EmbyUser> GetAllAdministrators();
        EmbyUser GetUserById(string id);
        Card<int> GetViewedEpisodeCountByUserId(string id);
        Card<int> GetViewedMovieCountByUserId(string id);
        IEnumerable<UserMediaView> GetUserViewPageByUserId(string id, int page, int size);
        int GetUserViewCount(string id);

        #endregion

        #region JobHelpers

        ServerInfo GetAndProcessServerInfo();
        void GetAndProcessPluginInfo();
        void GetAndProcessUsers();
        void GetAndProcessDevices();

        #endregion
    }
}
