using System;

namespace EmbyStat.Jobs;

public interface IJobInitializer
{
    void Setup(bool canUpdate);
    void UpdateTrigger(Guid id, string trigger, bool canUpdate);
}