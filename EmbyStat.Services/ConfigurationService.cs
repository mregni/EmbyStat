using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EmbyStat.Services
{
    public class ConfigurationService : IConfigurationService
    {
		private readonly IConfigurationRepository _configurationRepository;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly AppSettings _appSettings;

        public ConfigurationService(IConfigurationRepository configurationRepository, IStatisticsRepository statisticsRepository, IOptions<AppSettings> appSettings)
        {
            _configurationRepository = configurationRepository;
            _statisticsRepository = statisticsRepository;
            _appSettings = appSettings.Value;
        }

		public void SaveServerSettings(Configuration configuration)
		{
		    var oldConfig = _configurationRepository.GetConfiguration();
		    MarkMovieStatisticsAsInvalidIfNeeded(configuration, oldConfig);
            MarkShowStatisticsAsInvalidIfNeeded(configuration, oldConfig);

            _configurationRepository.Update(configuration);
        }

		public Configuration GetServerSettings()
		{
            var config = _configurationRepository.GetConfiguration();
            config.Version = _appSettings.Version;
            return config;
        }

        private void MarkMovieStatisticsAsInvalidIfNeeded(Configuration configuration, Configuration oldConfig)
        {
            if (!(oldConfig.MovieCollectionTypes.All(configuration.MovieCollectionTypes.Contains) &&
                  oldConfig.MovieCollectionTypes.Count == configuration.MovieCollectionTypes.Count))
            {
                _statisticsRepository.MarkMovieTypesAsInvalid();
            }
        }

        private void MarkShowStatisticsAsInvalidIfNeeded(Configuration configuration, Configuration oldConfig)
        {
            if (!(oldConfig.ShowCollectionTypes.All(configuration.ShowCollectionTypes.Contains) &&
                  oldConfig.ShowCollectionTypes.Count == configuration.ShowCollectionTypes.Count))
            {
                _statisticsRepository.MarkShowTypesAsInvalid();
            }
        }
    }
}
