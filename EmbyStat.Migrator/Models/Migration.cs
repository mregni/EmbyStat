using EmbyStat.Common.Models.Settings;
using EmbyStat.Migrator.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Migrator.Models;

public abstract class Migration : IMigration
{
    protected UserSettings UserSettings { get; set; }
    protected AppSettings AppSettings { get; set; }
    protected ISettingsService SettingsService { get; set; }
    public void MigrateUp(ISettingsService settingsService, long version)
    {
        SettingsService = settingsService;
        UserSettings = settingsService.GetUserSettings();
        AppSettings = settingsService.GetAppSettings();
        Up();

        settingsService.SaveUserSettingsAsync(UserSettings, version).Wait();
    }

    public abstract void Up();
}