using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.EmbyClient;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly ISettingsService _settingsService;
        private readonly IEmbyClient _embyClient;

        public PersonService(IPersonRepository personRepository, ISettingsService settingsService, IEmbyClient embyClient)
        {
            _personRepository = personRepository;
            _settingsService = settingsService;
            _embyClient = embyClient;
        }

        public async Task<Person> GetPersonById(string id)
        {
            var person = _personRepository.GetPersonById(id);
            if (!person.Synced)
            {
                var settings = _settingsService.GetUserSettings();
                var query = new ItemQuery { UserId = settings.Emby.UserId };

                _embyClient.SetAddressAndUrl(settings.FullEmbyServerAddress, settings.Emby.AccessToken);
                var rawPerson = await _embyClient.GetPersonByNameAsync(person.Name, CancellationToken.None);
                person = PersonConverter.ConvertToPerson(rawPerson);
                _personRepository.AddOrUpdatePerson(person);
            }

            return person;
        }
    }
}
