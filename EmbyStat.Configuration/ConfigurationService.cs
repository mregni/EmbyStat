using EmbyStat.Configuration.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EmbyStat.Configuration;

public class ConfigurationService : IConfigurationService
{
    private Config _config;

    public ConfigurationService(IOptions<Config> options)
    {
        _config = options.Value;
    }

    public Config Get()
    {
        return _config;
    }

    private async Task WriteConfiguration(Config config)
    {
        var path = Path.Combine(config.SystemConfig.Dirs.Config, "config.json");
        if (!File.Exists(path))
        {
            throw new InvalidConfigFileException($"Config file cannot be found at {path}");
        }
        
        var jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };
        jsonSettings.Converters.Add(new StringEnumConverter());

        var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings);
        await File.WriteAllTextAsync(path, newJson);
        
        _config = config;
    }

    public Task UpdateSystemConfiguration(SystemConfig config)
    {
        var localConfig = _config;
        localConfig.SystemConfig = config;
        return WriteConfiguration(localConfig);
    }

    public Task UpdateUserConfiguration(UserConfig config)
    {
        var localConfig = _config;
        localConfig.UserConfig = config;
        return WriteConfiguration(localConfig);
    }

    public Task SetUpdateInProgressSettingAsync(bool state)
    {
        var localConfig = _config;
        localConfig.SystemConfig.UpdateInProgress = state;
        return WriteConfiguration(localConfig);
    }
}