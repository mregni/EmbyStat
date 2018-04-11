using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace EmbyStat.Services.Interfaces
{
    public interface IPluginService
	{
		List<PluginInfo> GetInstalledPlugins();
	}
}
