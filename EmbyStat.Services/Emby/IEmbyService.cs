using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Services.Emby.Models;


namespace EmbyStat.Services.Emby
{
    public interface IEmbyService
	{
	    EmbyUdpBroadcast SearchEmby();
	    Task<EmbyToken> GetEmbyToken(EmbyLogin login);
		void UpdateServerInfo();

	}
}
