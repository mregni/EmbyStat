using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IServerInfoRepository
    {
	    ServerInfo GetSingleOrDefault();
	    void UpdateOrAdd(ServerInfo entity);
    }
}
