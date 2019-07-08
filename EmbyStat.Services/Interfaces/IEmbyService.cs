using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Emby;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IEmbyService : IDisposable
    {
        #region Server

        EmbyUdpBroadcast SearchEmby();
        Task<EmbyToken> GetEmbyToken(EmbyLogin login);
        Task<ServerInfo> GetServerInfo();
        EmbyStatus GetEmbyStatus();
        Task<string> PingEmbyAsync(string embyAddress, string accessToken, CancellationToken cancellationToken);
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

        Task GetAndProcessServerInfo(string embyAddress, string accessToken);
        Task GetAndProcessPluginInfo(string embyAddress, string settingsAccessToken);
        Task GetAndProcessEmbyUsers(string embyAddress, string settingsAccessToken);
        Task GetAndProcessDevices(string embyAddress, string settingsAccessToken);

        #endregion
    }
}
