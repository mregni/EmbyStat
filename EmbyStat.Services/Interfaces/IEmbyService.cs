using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Emby;
using MediaBrowser.Model.Plugins;

namespace EmbyStat.Services.Interfaces
{
    public interface IEmbyService : IDisposable
    {
	    EmbyUdpBroadcast SearchEmby();
	    Task<EmbyToken> GetEmbyToken(EmbyLogin login);
        Task<ServerInfo> GetServerInfo();
		List<Drive> GetLocalDrives();
		void FireSmallSyncEmbyServerInfo();
	    EmbyStatus GetEmbyStatus();
	    Task<string> PingEmbyAsync(CancellationToken cancellationToken);
        Task GetAndProcessServerInfo();
        Task GetAndProcessPluginInfo();
        Task GetAndProcessEmbyDriveInfo();
        Task GetAndProcessEmbyUsers();
        List<PluginInfo> GetAllPlugins();
        void ResetMissedPings();
        void IncreaseMissedPings();
    }
}
