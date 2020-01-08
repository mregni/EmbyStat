using System;
using System.Threading.Tasks;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using NLog;

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
            _logger = LogManager.GetCurrentClassLogger();

            var settings = settingsService.GetUserSettings();
            _httpClient = clientStrategy.CreateHttpClient(settings.MediaServer?.ServerType ?? ServerType.Emby);
        }

        public Person GetPersonByName(string name)
        {
            try
            {
                var person = _personRepository.GetPersonByName(name);
                if (person == null)
                {
                    var rawPerson = _httpClient.GetPersonByName(name);
                    person = PersonConverter.Convert(rawPerson);
                    _personRepository.Insert(person);
                }

                person.MovieCount = _movieRepository.GetMediaCountForPerson(person.Id);
                person.ShowCount = _showRepository.GetMediaCountForPerson(person.Id);

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
