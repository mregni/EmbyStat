using System.Linq;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class ServerInfoRepository : IServerInfoRepository
    {
	    public ServerInfo GetSingle()
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    return context.ServerInfo.Single();
		    }
	    }

	    public void UpdateOrAdd(ServerInfo entity)
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    var settings = context.ServerInfo.AsNoTracking().SingleOrDefault();

			    if (settings != null)
			    {
				    entity.Id = settings.Id;
				    context.Entry(entity).State = EntityState.Modified;
			    }
			    else
			    {
				    context.ServerInfo.Add(entity);
				}

			    context.SaveChanges();
			}
	    }
	}
}
