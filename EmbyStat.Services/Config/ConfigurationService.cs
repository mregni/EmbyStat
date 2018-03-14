using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EmbyStat.Repositories.Config;
using EmbyStat.Services.Emby.Models;
using Newtonsoft.Json;

namespace EmbyStat.Services.Config
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
			var dbSettings = _configurationRepository.GetSingle();

			dbSettings.Language = configuration.Language;
			dbSettings.AccessToken = configuration.AccessToken;
			dbSettings.EmbyServerAddress = configuration.EmbyServerAddress;
			dbSettings.EmbyUserName = configuration.EmbyUserName;
			dbSettings.Username = configuration.Username;
			dbSettings.WizardFinished = configuration.WizardFinished;

			_configurationRepository.Update(dbSettings);
		}

		public Configuration GetServerSettings()
		{
			return _configurationRepository.GetSingle();
		}
	}
}
