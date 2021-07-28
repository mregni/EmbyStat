using System.Collections.Generic;
using MediaBrowser.Common.Configuration;

namespace EmbyStat.Plugin.Configuration
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        public IEnumerable<ConfigurationStore> GetConfigurations()
        {
            return new[]
            {
                    new ConfigurationStore
                    {
                        ConfigurationType = typeof(EmbyStatConfiguration),
                        Key = "embystat"
                    }
                };
        }
    }
}
