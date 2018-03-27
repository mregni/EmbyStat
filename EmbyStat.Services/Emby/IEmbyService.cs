using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Repositories.EmbyServerInfo;
using EmbyStat.Services.Emby.Models;
using MediaBrowser.Model.Plugins;


namespace EmbyStat.Services.Emby
{
    public interface IPluginService
	{
	    EmbyUdpBroadcast SearchEmby();
	    Task<EmbyToken> GetEmbyToken(EmbyLogin login);
		List<PluginInfo> GetInstalledPlugins();
		ServerInfo GetServerInfo();
		void FireSmallSyncEmbyServerInfo();

	}
}
