using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories.Config
{
    public class PluginRepository : IConfigurationRepository
    {
		public Configuration GetSingle()
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    return context.Configuration.Single();
		    }
	    }

	    public void UpdateOrAdd(Configuration entity)
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    var settings = context.Configuration.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id);

			    if (settings != null)
			    {
				    context.Entry(entity).State = EntityState.Modified;
			    }
			    else
			    {
				    context.Configuration.Add(entity);
			    }

			    context.SaveChanges();
			}
	    }
	}
}
