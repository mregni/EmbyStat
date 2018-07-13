using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
		public Configuration GetConfiguration()
	    {
		    using (var context = new ApplicationDbContext())
		    {
		        return new Configuration(context.Configuration);
		    }
	    }

	    public void UpdateOrAdd(Configuration config)
	    {
		    using (var context = new ApplicationDbContext())
		    {
		        context.Configuration.RemoveRange(context.Configuration);
		        context.SaveChanges();
		        context.Configuration.AddRange(config.GetKeyValuePairs());
		        context.SaveChanges();
            }
	    }
	}
}
