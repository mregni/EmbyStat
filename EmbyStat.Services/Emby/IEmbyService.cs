using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Repositories.EmbyDrive;
using EmbyStat.Repositories.EmbyServerInfo;
using EmbyStat.Services.Emby.Models;
using MediaBrowser.Model.Plugins;


namespace EmbyStat.Services.Emby
{
    public interface IEmbyService
	{
	    EmbyUdpBroadcast SearchEmby();
	    Task<EmbyToken> GetEmbyToken(EmbyLogin login);
		ServerInfo GetServerInfo();
		List<Drives> GetLocalDrives();
		void FireSmallSyncEmbyServerInfo();
		Task PingEmby();
	}
}
