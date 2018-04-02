using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Repositories.EmbyHeartBeat
{
    public interface IEmbyHeartBeatRepository
    {
	    void SavePing(Ping ping);
	    List<Ping> GetLastPings(int take);
    }
}
