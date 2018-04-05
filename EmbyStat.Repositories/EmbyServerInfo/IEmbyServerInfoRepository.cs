namespace EmbyStat.Repositories.EmbyServerInfo
{
    public interface IEmbyServerInfoRepository
    {
	    ServerInfo GetSingle();
	    void UpdateOrAdd(ServerInfo entity);
    }
}
