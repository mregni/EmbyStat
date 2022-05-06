using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmbyStat.Migrator.Interfaces;

namespace EmbyStat.Migrator.Models;

public class AssemblyMigrationSourceItem : IMigrationSourceItem
{
    private readonly Assembly _assembly;

    public AssemblyMigrationSourceItem(Assembly assembly)
    {
        _assembly = assembly;
    }

    public IEnumerable<Type> MigrationTypeCandidates => _assembly.GetExportedTypes()
        .Where(t => typeof(IMigration).IsAssignableFrom(t) && !t.IsAbstract);
}