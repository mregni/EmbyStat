using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Clients.EmbyClient;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IEmbyClient _embyClient;

        public PersonService(IPersonRepository personRepository, IEmbyClient embyClient)
        {
            _personRepository = personRepository;
            _embyClient = embyClient;
        }

        public async Task<Person> GetPersonById(string id)
        {
            var person = _personRepository.GetPersonById(id);
            if (!person.Synced)
            {
                var rawPerson = await _embyClient.GetPersonByNameAsync(person.Name, CancellationToken.None);
                person = PersonConverter.ConvertToPerson(rawPerson);
                _personRepository.AddOrUpdatePerson(person);
            }

            return person;
        }
    }
}
