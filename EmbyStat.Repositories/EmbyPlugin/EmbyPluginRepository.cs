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
			    return context.Plugins.OrderBy(x => x.Name).ToList();
		    }
	    }

	    public void RemoveAllAndInsertPluginRange(List<PluginInfo> plugins)
	    {
			using (var context = new ApplicationDbContext())
			{
				context.RemoveRange(context.Plugins.ToList());
				context.SaveChanges();

				context.Plugins.AddRange(plugins);
				context.SaveChanges();
			}
		}
    }
}
