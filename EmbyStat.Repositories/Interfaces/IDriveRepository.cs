using System.Collections.Generic;
using EmbyStat.Clients.EmbyClient.Model;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IDriveRepository
    {
	    List<Drive> GetAll();
		void RemoveAllAndInsertDriveRange(List<Drive> drives);
    }
}
