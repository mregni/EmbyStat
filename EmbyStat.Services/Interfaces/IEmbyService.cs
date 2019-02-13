using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Emby;

namespace EmbyStat.Services.Interfaces
{
    public interface IEmbyService : IDisposable
    {
        #region Server

        EmbyUdpBroadcast SearchEmby();
        Task<EmbyToken> GetEmbyToken(EmbyLogin login);
        Task<ServerInfo> GetServerInfo();
        List<Drive> GetLocalDrives();
        EmbyStatus GetEmbyStatus();
        Task<string> PingEmbyAsync(string embyAddress, string accessToken, CancellationToken cancellationToken);
        void ResetMissedPings();
        void IncreaseMissedPings();

        #endregion

        #region Plugins

        List<PluginInfo> GetAllPlugins();

        #endregion

        #region Users

        IEnumerable<User> GetAllUsers();
        
        #endregion

        #region JobHelpers

        Task GetAndProcessServerInfo(string embyAddress, string accessToken);
        Task GetAndProcessPluginInfo(string embyAddress, string settingsAccessToken);
        Task GetAndProcessEmbyDriveInfo(string embyAddress, string settingsAccessToken);
        Task GetAndProcessEmbyUsers(string embyAddress, string settingsAccessToken);
        Task GetAndProcessDevices(string embyAddress, string settingsAccessToken);

        #endregion
    }
}
