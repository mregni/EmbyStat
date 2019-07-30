using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Migrator.Interfaces
{
    public interface IMigrationSourceItem
    {
        IEnumerable<Type> MigrationTypeCandidates { get; }
    }
}
