using System.Collections.Generic;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class ConfigurationService : IConfigurationService
    {
		private readonly IConfigurationRepository _configurationRepository;
		public ConfigurationService(IConfigurationRepository configurationRepository)
		{
			_configurationRepository = configurationRepository;
		}

		public void SaveServerSettings(Configuration configuration)
		{
		    var dbSettings = _configurationRepository.GetConfiguration();

		    dbSettings.Language = configuration.Language;
		    dbSettings.AccessToken = configuration.AccessToken;
		    dbSettings.EmbyServerAddress = configuration.EmbyServerAddress;
		    dbSettings.EmbyUserName = configuration.EmbyUserName;
		    dbSettings.Username = configuration.Username;
		    dbSettings.WizardFinished = configuration.WizardFinished;
		    dbSettings.EmbyUserId = configuration.EmbyUserId;
		    dbSettings.ToShortMovie = configuration.ToShortMovie;

		    _configurationRepository.UpdateOrAdd(dbSettings);
        }

		public Configuration GetServerSettings()
		{
			return _configurationRepository.GetConfiguration();
		}
	}
}
