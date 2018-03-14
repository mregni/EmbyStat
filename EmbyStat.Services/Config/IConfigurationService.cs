using EmbyStat.Services.Emby.Models;

namespace EmbyStat.Services.Config
{
    public interface IConfigurationService
    {
	    void SaveServerSettings(Repositories.Config.Configuration configuration);
	    Repositories.Config.Configuration GetServerSettings();
	}
}
