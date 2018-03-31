using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient.Model;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.System;
using MediaBrowser.Model.Users;

namespace EmbyStat.Api.EmbyClient
{
    public interface IEmbyClient : IDisposable
    {
	    void SetAddressAndUrl(string url, string token);
		Task<AuthenticationResult> AuthenticateUserAsync(string username, string password, string address);
		Task<List<PluginInfo>> GetInstalledPluginsAsync();
		Task<SystemInfo> GetServerInfo();
	    Task<List<Drive>> GetLocalDrives();
    }
}
