using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Migrator.Interfaces;

namespace EmbyStat.Migrator.Models;

public abstract class Migration : IMigration
{
    protected Config Configuration { get; set; }
    protected IConfigurationService ConfigurationService { get; set; }
    public void MigrateUp(IConfigurationService configurationService, long version)
    {
        ConfigurationService = configurationService;
        Configuration = configurationService.Get();
        Up();

        Configuration.SystemConfig.Migration = version;
        configurationService.UpdateSystemConfiguration(Configuration.SystemConfig).Wait();
    }

    public abstract void Up();
}