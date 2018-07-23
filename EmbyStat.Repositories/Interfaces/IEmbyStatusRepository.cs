using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IEmbyStatusRepository
    {
        EmbyStatus GetEmbyStatus();
        void IncreaseMissedPings();
        void ResetMissedPings();
    }
}
