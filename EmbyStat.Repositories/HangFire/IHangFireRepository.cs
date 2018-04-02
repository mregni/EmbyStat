using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Repositories.HangFire
{
    public interface IHangFireRepository
    {
	    List<Set> GetSets();
	    List<Hash> GetHashesFromSet(string key, string value);

    }
}
