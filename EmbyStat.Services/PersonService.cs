using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IShowRepository _showRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IEmbyClient _embyClient;

        public PersonService(IPersonRepository personRepository, IShowRepository showRepository, IMovieRepository movieRepository, IEmbyClient embyClient)
        {
            _personRepository = personRepository;
            _movieRepository = movieRepository;
            _showRepository = showRepository;
            _embyClient = embyClient;
        }

        public async Task<Person> GetPersonByNameAsync(string name)
        {
             var person = _personRepository.GetPersonByName(name);
            if (person == null)
            {
                var rawPerson = await _embyClient.GetPersonByNameAsync(name, CancellationToken.None);
                person = PersonConverter.Convert(rawPerson);
            }

            person.MovieCount = _movieRepository.GetMovieCountForPerson(person.Id);
            person.ShowCount = _showRepository.GetShowCountForPerson(person.Id);

            return person;
        }
    }
}
