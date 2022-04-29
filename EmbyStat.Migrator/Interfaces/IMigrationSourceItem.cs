using System;
using System.Collections.Generic;

namespace EmbyStat.Migrator.Interfaces;

public interface IMigrationSourceItem
{
    IEnumerable<Type> MigrationTypeCandidates { get; }
}