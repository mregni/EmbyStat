using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Users;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IMediaServerService
    {
        #region Server
        Task<IEnumerable<MediaServerUdpBroadcast>> SearchMediaServer(ServerType type);
        Task<SqlServerInfo> GetServerInfo(bool forceReSync);
        bool TestNewApiKey(string url, string apiKey, ServerType type);
        EmbyStatus GetMediaServerStatus();
        bool PingMediaServer(string url);
        void ResetMissedPings();
        void IncreaseMissedPings();
        void ResetMediaServerData();
        IEnumerable<Library> GetMediaServerLibraries();

        #endregion

        #region Plugins

        Task<List<SqlPluginInfo>> GetAllPlugins();

        #endregion

        #region Users

        Task<List<SqlUser>>  GetAllUsers();
        Task<List<SqlUser>>  GetAllAdministrators();
        EmbyUser GetUserById(string id);
        Card<int> GetViewedEpisodeCountByUserId(string id);
        Card<int> GetViewedMovieCountByUserId(string id);
        IEnumerable<UserMediaView> GetUserViewPageByUserId(string id, int page, int size);
        int GetUserViewCount(string id);

        #endregion

        #region Devices

        Task<List<SqlDevice>> GetAllDevices();

        #endregion

        #region JobHelpers

        Task<SqlServerInfo> GetAndProcessServerInfo();
        Task GetAndProcessPluginInfo();
        Task GetAndProcessUsers();
        Task GetAndProcessDevices();

        #endregion
    }
}
