using System.Collections.Generic;
using EmbyStat.Api.EmbyClient.Model;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IDriveRepository
    {
	    List<Drive> GetAll();
		void ClearAndInsertList(List<Drive> drives);
    }
}
