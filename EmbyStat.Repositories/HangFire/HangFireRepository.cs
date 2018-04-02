using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbyStat.Repositories.HangFire
{
    public class HangFireRepository : IHangFireRepository
    {
	    public List<Set> GetSets()
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    return context.Sets.ToList();
		    }
	    }

	    public List<Hash> GetHashesFromSet(string key, string value)
	    {
		    using (var context = new ApplicationDbContext())
		    {
				return context.Hashes.Where(x => x.Key == $"{key}:{value}").ToList();
		    }
	    }
    }
}
