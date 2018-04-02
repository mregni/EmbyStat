using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbyStat.Repositories.EmbyHeartBeat
{
    public class EmbyHeartBeatRepository : IEmbyHeartBeatRepository
    {
	    public void SavePing(Ping ping)
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    context.Pings.Add(ping);
			    context.SaveChanges();
		    }
	    }

	    public List<Ping> GetLastPings(int take)
	    {
			using (var context = new ApplicationDbContext())
			{
				return context.Pings.OrderByDescending(x => x.Time).Take(take).ToList();
			}
		}
    }
}
