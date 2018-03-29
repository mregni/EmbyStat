using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Repositories.EmbyServerInfo
{
    public interface IEmbyServerInfoRepository
    {
	    ServerInfo GetSingle();
	    void UpdateOrAdd(ServerInfo entity);
    }
}
