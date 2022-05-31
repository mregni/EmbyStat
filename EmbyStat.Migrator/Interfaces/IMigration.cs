using EmbyStat.Configuration.Interfaces;

namespace EmbyStat.Migrator.Interfaces;

public interface IMigration
{
    void MigrateUp(IConfigurationService configurationService, long version);
}