using System.Collections.Generic;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IDriveRepository
    {
	    List<Drives> GetAll();
		void ClearAndInsertList(List<Drives> drives);
    }
}
