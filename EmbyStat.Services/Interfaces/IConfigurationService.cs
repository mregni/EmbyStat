using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces
{
    public interface IConfigurationService
    {
	    void SaveServerSettings(Configuration configuration);
        Configuration GetServerSettings();
    }
}
