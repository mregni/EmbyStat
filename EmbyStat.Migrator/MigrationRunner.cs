using System;
using System.Collections.Generic;
using System.Reflection;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Migrator.Interfaces;
using EmbyStat.Migrator.Models;

namespace EmbyStat.Migrator;

public class MigrationRunner : IMigrationRunner
{
    private readonly IEnumerable<Type> _migrations;
    private readonly IConfigurationService _configurationService;

    public MigrationRunner(IMigrationSourceItem source, IConfigurationService configurationService)
    {
        _migrations = source.MigrationTypeCandidates;
        _configurationService = configurationService;
    }

    public void Migrate()
    {
        var config = _configurationService.Get();
        var orderedMigrations = GetOrderedMigrations(config.SystemConfig.Migration);
        StartMigrations(orderedMigrations);
    }

    private SortedList<long, Type> GetOrderedMigrations(long settingsVersion)
    {
        var orderedList = new SortedList<long, Type>();
        foreach (var migration in _migrations)
        {
            var migrationAttribute = migration.GetCustomAttribute<MigrationAttribute>();
            if (migrationAttribute?.Version > settingsVersion)
            {
                orderedList.Add(migrationAttribute.Version, migration);
            }
        }

        return orderedList;
    }

    private void StartMigrations(SortedList<long, Type> migrations)
    {
        foreach (var (key, value) in migrations)
        {
            var migration = (IMigration)Activator.CreateInstance(value);
            migration?.MigrateUp(_configurationService, key);
        }
    }
}