using System;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Logging;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IShowRepository _showRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IHttpClient _httpClient;
        private readonly Logger _logger;

        public PersonService(IPersonRepository personRepository, IShowRepository showRepository, IMovieRepository movieRepository, 
            IClientStrategy clientStrategy, ISettingsService settingsService)
        {
            _personRepository = personRepository;
            _movieRepository = movieRepository;
            _showRepository = showRepository;
            _logger = LogFactory.CreateLoggerForType(typeof(PersonService), "PERSON-SERVICE");

            var settings = settingsService.GetUserSettings();
            _httpClient = clientStrategy.CreateHttpClient(settings.MediaServer?.ServerType ?? ServerType.Emby);
        }

        private Person GetPersonByName(string name)
        {
            try
            {
                var person = _personRepository.GetPersonByName(name);
                if (person == null)
                {
                    person = _httpClient.GetPersonByName(name);

                    if (person != null)
                    {
                        _personRepository.Upsert(person);
                    }
                    else
                    {
                        person = new Person
                        {
                            Name = name
                        };
                    }
                }

                return person;
            }
            catch (Exception e)
            {
                _logger.Warn(e, $"Error fetching data for person {name}.");
                return null;
            }
        }

        public Person GetPersonByNameForMovies(string name)
        {
            var person = GetPersonByName(name);
            if (person != null)
            {
                person.MovieCount = _movieRepository.GetMediaCountForPerson(person.Name);
            }

            return person;
        }

        public Person GetPersonByNameForMovies(string name, string genre)
        {
            var person = GetPersonByName(name);
            if (person != null)
            {
                person.MovieCount = _movieRepository.GetMediaCountForPerson(person.Name, genre);
            }

            return person;
        }

        public Person GetPersonByNameForShows(string name, string genre)
        {
            var person = GetPersonByName(name);
            person.ShowCount = _showRepository.GetMediaCountForPerson(person.Name, genre);

            return person;
        }
    }
}
