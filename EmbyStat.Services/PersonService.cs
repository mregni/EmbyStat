using System;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using NLog;
using NLog.Fluent;

namespace EmbyStat.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IShowRepository _showRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IEmbyClient _embyClient;
        private readonly Logger _logger;

        public PersonService(IPersonRepository personRepository, IShowRepository showRepository, IMovieRepository movieRepository, IEmbyClient embyClient)
        {
            _personRepository = personRepository;
            _movieRepository = movieRepository;
            _showRepository = showRepository;
            _embyClient = embyClient;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<Person> GetPersonByNameAsync(string name)
        {
            try
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
            catch (Exception e)
            {
                _logger.Warn(e, $"Error fetching data for person {name}.");
                return null;
            }
        }
    }
}
