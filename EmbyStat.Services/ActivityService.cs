using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class ActivityService: IActivityService
    {
        private readonly IEmbyClient _embyClient;
        private readonly IConfigurationRepository _configurationRepository;

        public ActivityService(IEmbyClient embyClient)
        {
            _embyClient = embyClient;
        }

        public IEnumerable<UserActivity> GetLastUserActivities()
        {
            var settings = _configurationRepository.GetConfiguration();

            _embyClient.SetAddressAndUrl(settings.EmbyServerAddress, settings.AccessToken);
            
            return null;
        }
    }
}
