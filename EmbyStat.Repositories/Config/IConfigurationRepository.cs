namespace EmbyStat.Repositories.Config
{
    public interface IConfigurationRepository
    {
	    Configuration GetSingle();
	    void UpdateOrAdd(Configuration entity);
	}
}
