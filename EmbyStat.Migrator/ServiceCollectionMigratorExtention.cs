using System;
using System.Reflection;
using EmbyStat.Migrator.Interfaces;
using EmbyStat.Migrator.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmbyStat.Migrator;

public static class ServiceCollectionMigratorExtension
{
    public static IServiceCollection AddJsonMigrator(this IServiceCollection services, Assembly assembly)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.TryAddSingleton<IMigrationRunner, MigrationRunner>();
        services.TryAddSingleton<IMigrationSourceItem>(new AssemblyMigrationSourceItem(assembly));

        return services;
    }
}