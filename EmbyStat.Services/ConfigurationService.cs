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

		public void SaveServerSettings(Dictionary<string, string> configuration)
		{
		    var dbSettings = _configurationRepository.GetConfiguration();

			dbSettings[Constants.Configuration.Language] = configuration[Constants.Configuration.Language];
		    dbSettings[Constants.Configuration.AccessToken] = configuration[Constants.Configuration.AccessToken];
		    dbSettings[Constants.Configuration.EmbyServerAddress] = configuration[Constants.Configuration.EmbyServerAddress];
		    dbSettings[Constants.Configuration.EmbyUserName] = configuration[Constants.Configuration.EmbyUserName];
		    dbSettings[Constants.Configuration.UserName] = configuration[Constants.Configuration.UserName];
		    dbSettings[Constants.Configuration.WizardFinished] = configuration[Constants.Configuration.WizardFinished];
		    dbSettings[Constants.Configuration.EmbyUserId] = configuration[Constants.Configuration.EmbyUserId];
		    dbSettings[Constants.Configuration.ToShortMovie] = configuration[Constants.Configuration.ToShortMovie];

			_configurationRepository.UpdateOrAdd(dbSettings);
		}

		public Dictionary<string, string> GetServerSettings()
		{
			return _configurationRepository.GetConfiguration();
		}
	}
}
