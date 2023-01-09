namespace EmbyStat.Configuration.Interfaces;

public interface IConfigurationService
{
    Config Get();
    TimeZoneInfo GetLocalTimeZoneInfo();
    Task UpdateSystemConfiguration(SystemConfig config);
    Task UpdateUserConfiguration(UserConfig config);
    Task SetUpdateInProgressSettingAsync(bool state);
}