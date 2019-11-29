using System;

namespace EmbyStat.Jobs
{
    public interface IJobInitializer
    {
        void Setup(bool disableUpdates);
        void UpdateTrigger(Guid id, string trigger, bool disableUpdates);
    }
}