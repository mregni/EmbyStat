using System;

namespace EmbyStat.Migrator.Models;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MigrationAttribute : Attribute
{
    public readonly int Version;
    public MigrationAttribute(int version)
    {
        Version = version;
    }
}