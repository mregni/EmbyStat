using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IMediaServerService
    {
        #region Server

        Task<IEnumerable<MediaServerUdpBroadcast>> SearchMediaServer(ServerType type);
        Task<MediaServerInfo> GetServerInfo(bool forceReSync);
        Task<bool> TestNewApiKey(string url, string apiKey, ServerType type);
        Task<MediaServerStatus> GetMediaServerStatus();
        Task<bool> PingMediaServer(string url);
        Task<bool> PingMediaServer();
        Task ResetMissedPings();
        Task IncreaseMissedPings();
        Task<Library[]> GetMediaServerLibraries();

        #endregion

        #region Plugins

        Task<List<PluginInfo>> GetAllPlugins();

        #endregion

        #region Users

        Task<List<MediaServerUser>> GetAllUsers();
        Task<List<MediaServerUser>> GetAllAdministrators();
        EmbyUser GetUserById(string id);
        Card<int> GetViewedEpisodeCountByUserId(string id);
        Card<int> GetViewedMovieCountByUserId(string id);
        IEnumerable<UserMediaView> GetUserViewPageByUserId(string id, int page, int size);
        int GetUserViewCount(string id);

        #endregion

        #region Devices

        Task<List<Device>> GetAllDevices();

        #endregion

        #region JobHelpers

        Task<MediaServerInfo> GetAndProcessServerInfo();
        Task GetAndProcessPluginInfo();
        Task GetAndProcessUsers();
        Task GetAndProcessDevices();
        Task GetAndProcessLibraries();

        #endregion
    }
}