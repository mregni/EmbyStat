using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IServerInfoRepository
    {
	    ServerInfo GetSingle();
	    void UpdateOrAdd(ServerInfo entity);
    }
}
