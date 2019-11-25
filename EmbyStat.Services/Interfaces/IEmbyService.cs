using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IEmbyService
    {
        #region Server

        EmbyUdpBroadcast SearchEmby();
        Task<ServerInfo> GetServerInfoAsync();
        Task<bool> TestNewEmbyApiKeyAsync(string url, string apiKey);
        EmbyStatus GetEmbyStatus();
        Task<string> PingEmbyAsync(string url);
        void ResetMissedPings();
        void IncreaseMissedPings();

        #endregion

        #region Plugins

        List<PluginInfo> GetAllPlugins();

        #endregion

        #region Users

        IEnumerable<EmbyUser> GetAllUsers();
        EmbyUser GetUserById(string id);
        Card<int> GetViewedEpisodeCountByUserId(string id);
        Card<int> GetViewedMovieCountByUserId(string id);
        IEnumerable<UserMediaView> GetUserViewPageByUserId(string id, int page, int size);
        int GetUserViewCount(string id);

        #endregion

        #region JobHelpers

        Task<ServerInfo> GetAndProcessServerInfoAsync();
        Task GetAndProcessPluginInfoAsync();
        Task GetAndProcessEmbyUsersAsync();
        Task GetAndProcessDevicesAsync();

        #endregion
    }
}
