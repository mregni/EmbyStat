namespace EmbyStat.Repositories.Config
{
    public interface IConfigurationRepository
    {
	    Configuration GetSingle();
	    void Update(Configuration entity);
	}
}
