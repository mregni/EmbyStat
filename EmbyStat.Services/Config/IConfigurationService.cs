using EmbyStat.Services.Config.Models;

namespace EmbyStat.Services.Config
{
    public interface IConfigurationService
    {
	    void SaveServerSettings(Repositories.Config.Configuration configuration);
	    void SaveEmbySettings(EmbySettings configuration);
	    EmbyUdpBroadcast SearchEmby();
	    Repositories.Config.Configuration GetServerSettings();
	}
}
