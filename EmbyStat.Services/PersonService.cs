using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IEmbyClient _embyClient;

        public PersonService(IPersonRepository personRepository, IConfigurationRepository configurationRepository, IEmbyClient embyClient)
        {
            _personRepository = personRepository;
            _configurationRepository = configurationRepository;
            _embyClient = embyClient;
        }

        public async Task<Person> GetPersonById(string id)
        {
            var person = _personRepository.GetPersonById(id);
            if (person == null || !person.Synced)
            {
                var settings = _configurationRepository.GetConfiguration();
                var query = new ItemQuery { UserId = settings.EmbyUserId };

                _embyClient.SetAddressAndUrl(settings.GetFullEmbyServerAddress(), settings.AccessToken);
                var rawPerson = await _embyClient.GetItemAsync(query, id, CancellationToken.None);
                person = PersonHelper.ConvertToPerson(rawPerson);
                _personRepository.AddOrUpdatePerson(person);
            }

            return person;
        }
    }
}
