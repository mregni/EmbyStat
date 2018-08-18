using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class DriveRepository : IDriveRepository
    {
	    public List<Drives> GetAll()
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    return context.Drives.ToList();
		    }
	    }

	    public void ClearAndInsertList(List<Drives> drives)
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    context.RemoveRange(context.Drives.ToList());
			    context.SaveChanges();

			    context.AddRange(drives);
			    context.SaveChanges();
		    }
	    }
    }
}
