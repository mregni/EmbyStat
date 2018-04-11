using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IPluginRepository
    {
	    List<PluginInfo> GetPlugins();
	    void RemoveAllAndInsertPluginRange(List<PluginInfo> plugins);
    }
}
