using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IEmbyStatusRepository
    {
        EmbyStatus GetEmbyStatus();
        void IncreaseMissedPings();
        void ResetMissedPings();
    }
}
