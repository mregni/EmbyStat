using MediaBrowser.Common.Configuration;

namespace EmbyStat.Plugin.Configuration
{
    public static class ConfigurationExtension
    {
        public static EmbyStatConfiguration GetEmbyStatConfiguration(this IConfigurationManager manager)
        {
            return manager.GetConfiguration<EmbyStatConfiguration>("embystat");
        }

        public static void SaveEmbyStatConfiguration(this IConfigurationManager manager, EmbyStatConfiguration configuration)
        {
            manager.SaveConfiguration("embystat", configuration);
        }
    }
}
