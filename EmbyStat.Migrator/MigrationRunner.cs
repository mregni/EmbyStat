﻿using System;
using System.Collections.Generic;
using System.Reflection;
using EmbyStat.Migrator.Interfaces;
using EmbyStat.Migrator.Models;
using EmbyStat.Services.Interfaces;

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
            var settingsVersion = _settingsService.GetUserSettingsVersion();
            var orderedMigrations = GetOrderedMigrations(settingsVersion);
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
                migration?.MigrateUp(_settingsService, key);
            }
        }
    }
}
