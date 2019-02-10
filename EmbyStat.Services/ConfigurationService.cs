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
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly AppSettings _appSettings;

        public ConfigurationService(IStatisticsRepository statisticsRepository, IOptions<AppSettings> appSettings)
        {
            _statisticsRepository = statisticsRepository;
            _appSettings = appSettings.Value;
        }

        private void MarkMovieStatisticsAsInvalidIfNeeded(UserSettings configuration, UserSettings oldConfig)
        {
            if (!(oldConfig.MovieCollectionTypes.All(configuration.MovieCollectionTypes.Contains) &&
                  oldConfig.MovieCollectionTypes.Count == configuration.MovieCollectionTypes.Count))
            {
                _statisticsRepository.MarkMovieTypesAsInvalid();
            }
        }

        private void MarkShowStatisticsAsInvalidIfNeeded(UserSettings configuration, UserSettings oldConfig)
        {
            if (!(oldConfig.ShowCollectionTypes.All(configuration.ShowCollectionTypes.Contains) &&
                  oldConfig.ShowCollectionTypes.Count == configuration.ShowCollectionTypes.Count))
            {
                _statisticsRepository.MarkShowTypesAsInvalid();
            }
        }
    }
}
