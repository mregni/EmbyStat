using EmbyStat.Services.Interfaces;

namespace EmbyStat.Migrator.Interfaces;

public interface IMigration
{
    void MigrateUp(ISettingsService settingsService, long version);
}