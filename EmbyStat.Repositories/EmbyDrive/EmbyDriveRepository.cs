using System.Collections.Generic;
using System.Linq;

namespace EmbyStat.Repositories.EmbyDrive
{
    public class EmbyDriveRepository : IEmbyDriveRepository
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
