using EmbyStat.Repositories.Config;

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

			_configurationRepository.UpdateOrAdd(dbSettings);
		}

		public Configuration GetServerSettings()
		{
			return _configurationRepository.GetSingle();
		}
	}
}
