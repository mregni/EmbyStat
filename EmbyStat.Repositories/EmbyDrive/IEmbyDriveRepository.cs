using System.Collections.Generic;

namespace EmbyStat.Repositories.EmbyDrive
{
    public interface IEmbyDriveRepository
    {
	    List<Drives> GetAll();
		void ClearAndInsertList(List<Drives> drives);
    }
}
