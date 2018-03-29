using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace EmbyStat.Services.Plugin
{
    public interface IPluginService
	{
		List<PluginInfo> GetInstalledPlugins();
	}
}
