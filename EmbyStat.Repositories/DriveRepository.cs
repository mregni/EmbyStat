using System.Collections.Generic;
using System.Linq;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class DriveRepository : IDriveRepository
    {
	    public List<Drive> GetAll()
	    {
		    using (var context = new ApplicationDbContext())
		    {
			    return context.Drives.ToList();
		    }
	    }

	    public void RemoveAllAndInsertDriveRange(List<Drive> drives)
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
