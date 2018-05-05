using System.Linq;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
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
