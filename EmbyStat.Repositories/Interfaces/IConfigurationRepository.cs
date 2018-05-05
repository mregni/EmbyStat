using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IConfigurationRepository
    {
	    Configuration GetSingle();
	    void UpdateOrAdd(Configuration entity);
	}
}
