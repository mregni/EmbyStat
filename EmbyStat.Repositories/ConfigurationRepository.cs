using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
		public Dictionary<string, string> GetConfiguration()
	    {
		    using (var context = new ApplicationDbContext())
		    {
		        return context.Configuration.ToDictionary(x => x.Id, y => y.Value);
		    }
	    }

	    public void UpdateOrAdd(Dictionary<string, string> entities)
	    {
		    using (var context = new ApplicationDbContext())
		    {
		        foreach (var entity in entities)
		        {
		            var config = context.Configuration.SingleOrDefault(x => x.Id == entity.Key);

		            if (config != null)
		            {
		                config.Value = entity.Value;
		            }
		            else
		            {
		                context.Configuration.Add(new Configuration { Id = entity.Key, Value = entity.Value });
		            }
                }
			    
			    context.SaveChanges();
			}
	    }
	}
}
