using System;

namespace EmbyStat.Jobs
{
    public interface IJobInitializer
    {
        void Setup();
        void UpdateTrigger(Guid id, string trigger);
    }
}