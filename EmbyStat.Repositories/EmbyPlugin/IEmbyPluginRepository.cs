using System;
using System.Collections.Generic;
using System.Text;
using MediaBrowser.Model.Plugins;

namespace EmbyStat.Repositories.EmbyPlugin
{
    public interface IEmbyPluginRepository
    {
	    List<PluginInfo> GetPlugins();
	    void InsertPlugin(PluginInfo plugin);
	    void InsertPluginRange(List<PluginInfo> plugins);
    }
}
