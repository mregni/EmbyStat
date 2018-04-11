using EmbyStat.Common.Models;

namespace EmbyStat.Services.Interfaces
{
    public interface IConfigurationService
    {
	    void SaveServerSettings(Configuration configuration);
	    Configuration GetServerSettings();
	}
}
