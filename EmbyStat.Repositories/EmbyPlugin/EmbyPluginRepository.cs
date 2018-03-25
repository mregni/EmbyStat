using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.Plugins;

namespace EmbyStat.Repositories.EmbyPlugin
{
    public class EmbyPluginRepository : IEmbyPluginRepository
    {
	    public List<PluginInfo> GetPlugins()
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    return context.Plugins.ToList();
		    }
	    }

	    public void InsertPlugin(PluginInfo plugin)
	    {
			using (var context = new ApplicationDbContext())
			{
				context.Plugins.Add(plugin);
				context.SaveChanges();
			}
		}

	    public void InsertPluginRange(List<PluginInfo> plugins)
	    {
			using (var context = new ApplicationDbContext())
			{
				context.Plugins.AddRange(plugins);
				context.SaveChanges();
			}
		}
    }
}
