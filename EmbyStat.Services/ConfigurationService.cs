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
		    _configurationRepository.Update(configuration);
        }

		public Configuration GetServerSettings()
		{
			return _configurationRepository.GetConfiguration();
		}
	}
}
