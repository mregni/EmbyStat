using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class ConfigurationService : IConfigurationService
    {
		private readonly IConfigurationRepository _configurationRepository;
        private readonly IStatisticsRepository _statisticsRepository;

        public ConfigurationService(IConfigurationRepository configurationRepository, IStatisticsRepository statisticsRepository)
        {
            _configurationRepository = configurationRepository;
            _statisticsRepository = statisticsRepository;
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
			return _configurationRepository.GetConfiguration();
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
