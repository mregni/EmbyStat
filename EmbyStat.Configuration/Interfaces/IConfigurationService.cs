namespace EmbyStat.Configuration.Interfaces;

public interface IConfigurationService
{
    Config Get();
    Task UpdateSystemConfiguration(SystemConfig config);
    Task UpdateUserConfiguration(UserConfig config);
    Task SetUpdateInProgressSettingAsync(bool state);
}