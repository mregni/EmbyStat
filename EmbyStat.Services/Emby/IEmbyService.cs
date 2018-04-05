using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Repositories.EmbyDrive;
using EmbyStat.Repositories.EmbyServerInfo;
using EmbyStat.Services.Emby.Models;


namespace EmbyStat.Services.Emby
{
    public interface IEmbyService
	{
	    EmbyUdpBroadcast SearchEmby();
	    Task<EmbyToken> GetEmbyToken(EmbyLogin login);
		ServerInfo GetServerInfo();
		List<Drives> GetLocalDrives();
		void FireSmallSyncEmbyServerInfo();
	}
}
