using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace EmbyStat.Repositories.EmbyPlugin
{
    public interface IEmbyPluginRepository
    {
	    List<PluginInfo> GetPlugins();
	    void RemoveAllAndInsertPluginRange(List<PluginInfo> plugins);
    }
}
