using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IServerInfoRepository
    {
	    ServerInfo GetSingle();
	    void UpdateOrAdd(ServerInfo entity);
    }
}
