using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EmbyStat.Migrator.Interfaces;
using EmbyStat.Migrator.Models;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EmbyStat.Migrator
{
    public class MigrationRunner : IMigrationRunner
    {
        private readonly IEnumerable<Type> _migrations;
        private readonly ISettingsService _settingsService;

        public MigrationRunner(IMigrationSourceItem source, ISettingsService settingsService)
        {
            _migrations = source.MigrationTypeCandidates;
            _settingsService = settingsService;
        }

        public void Migrate()
        {
            _settingsService.LoadUserSettingsFromFile();
            long settingsVersion = _settingsService.GetUserSettingsVersion();
            var orderedMigrations = GetOrderedMigrations(settingsVersion);
            StartMigrations(orderedMigrations);
        }

        private SortedList<long, Type> GetOrderedMigrations(long settingsVersion)
        {
            var orderedList = new SortedList<long, Type>();
            foreach (var migration in _migrations)
            {
                var migrationAttribute = migration.GetCustomAttribute<MigrationAttribute>();
                if (migrationAttribute.Version > settingsVersion)
                {
                    orderedList.Add(migrationAttribute.Version, migration);
                }
            }

            return orderedList;
        }

        private void StartMigrations(SortedList<long, Type> migrations)
        {
            foreach (var type in migrations)
            {
                var migration = (IMigration)Activator.CreateInstance(type.Value);
                migration.MigrateUp(_settingsService, type.Key);
            }
        }
    }
}
