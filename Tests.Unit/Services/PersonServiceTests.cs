using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    [Collection("Services collection")]
    public class PersonServiceTests
    {
        private readonly PersonService _subject;
        private readonly Mock<IPersonRepository> _personREpositoryMock;
        private readonly Mock<IConfigurationRepository> _configurationRepositoryMock;
        private readonly Mock<IEmbyClient> _embyClientMock;
        private Person returnedPerson;
        private BaseItemDto basePerson;
        public PersonServiceTests()
        {
            var configuration = new List<ConfigurationKeyValue>
            {
                new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserId, Value = "EmbyUserId" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.Language, Value = "en-US" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.UserName, Value = "admin" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.WizardFinished, Value = "true" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerAddress, Value = "localhost" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.AccessToken, Value = "1234567980" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyUserName, Value = "reggi" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.ToShortMovie, Value = "10" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.ServerName, Value = "ServerName" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerProtocol, Value = "0" },
                new ConfigurationKeyValue{ Id = Constants.Configuration.EmbyServerPort, Value = "8096" }
            };

            basePerson = new BaseItemDto()
            {
                Id = Guid.NewGuid(),
                Name = "name",
                ImageTags = new Dictionary<ImageType, string> {{ImageType.Primary, ""}},
                MovieCount = 10,
                PremiereDate = new DateTime(2000, 1, 1),
                ChildCount = 20,
                EpisodeCount = 10,
                Etag = "etag",
                //HomePageUrl = "localhost.be",
                ProviderIds = new Dictionary<string, string> { { "Imdb", "12345"}, { "Tmdb", "12345"} },
                Overview = "Lots of text",
                SeriesCount = 1,
                SortName = "name"
            };

            _personREpositoryMock = new Mock<IPersonRepository>();
            _personREpositoryMock.Setup(x => x.AddOrUpdatePerson(It.IsAny<Person>()));

            _configurationRepositoryMock = new Mock<IConfigurationRepository>();
            _configurationRepositoryMock.Setup(x => x.GetConfiguration()).Returns(new Configuration(configuration));

            _embyClientMock = new Mock<IEmbyClient>();
            _embyClientMock.Setup(x => x.GetItemAsync(It.IsAny<ItemQuery>(), It.IsAny<Guid>(), CancellationToken.None))
                .Returns(Task.FromResult(basePerson));

            _subject = new PersonService(_personREpositoryMock.Object, _configurationRepositoryMock.Object, _embyClientMock.Object);
        }

        [Fact]
        public async void GetPersonByIdShouldGoToEmby()
        {
            returnedPerson = null;
            _personREpositoryMock.Setup(x => x.GetPersonById(It.IsAny<Guid>())).Returns(returnedPerson);

            var person = await _subject.GetPersonById(Guid.NewGuid());

            person.Should().NotBeNull();
            person.Id.Should().Be(basePerson.Id);
            person.Name.Should().Be(basePerson.Name);
            person.Primary.Should().Be(basePerson.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            person.MovieCount.Should().Be(basePerson.MovieCount);
            person.BirthDate.Should().Be(basePerson.PremiereDate);
            person.ChildCount.Should().Be(basePerson.ChildCount);
            person.EpisodeCount.Should().Be(basePerson.EpisodeCount);
            person.Etag.Should().Be(basePerson.Etag);
            //person.HomePageUrl.Should().Be(basePerson.HomePageUrl);
            person.IMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value);
            person.TMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value);
            person.OverView.Should().Be(basePerson.Overview);
            person.SeriesCount.Should().Be(basePerson.SeriesCount);
            person.SortName.Should().Be(basePerson.SortName);
            person.Synced.Should().BeTrue();

            _personREpositoryMock.Verify(x => x.GetPersonById(It.IsAny<Guid>()), Times.Once);
            _personREpositoryMock.Verify(x => x.AddOrUpdatePerson(It.IsAny<Person>()), Times.Once);

            _configurationRepositoryMock.Verify(x => x.GetConfiguration(), Times.Once);

            _embyClientMock.Verify(x => x.GetItemAsync(It.IsAny<ItemQuery>(), It.IsAny<Guid>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async void GetPersonByIdNotSyncedPerson()
        {
            returnedPerson = new Person
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Synced = false
            };
            _personREpositoryMock.Setup(x => x.GetPersonById(It.IsAny<Guid>())).Returns(returnedPerson);

            var person = await _subject.GetPersonById(Guid.NewGuid());

            person.Should().NotBeNull();
            person.Id.Should().Be(basePerson.Id);
            person.Name.Should().Be(basePerson.Name);
            person.Primary.Should().Be(basePerson.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            person.MovieCount.Should().Be(basePerson.MovieCount);
            person.BirthDate.Should().Be(basePerson.PremiereDate);
            person.ChildCount.Should().Be(basePerson.ChildCount);
            person.EpisodeCount.Should().Be(basePerson.EpisodeCount);
            person.Etag.Should().Be(basePerson.Etag);
           // person.HomePageUrl.Should().Be(basePerson.HomePageUrl);
            person.IMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value);
            person.TMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value);
            person.OverView.Should().Be(basePerson.Overview);
            person.SeriesCount.Should().Be(basePerson.SeriesCount);
            person.SortName.Should().Be(basePerson.SortName);
            person.Synced.Should().BeTrue();

            _personREpositoryMock.Verify(x => x.GetPersonById(It.IsAny<Guid>()), Times.Once);
            _personREpositoryMock.Verify(x => x.AddOrUpdatePerson(It.IsAny<Person>()), Times.Once);

            _configurationRepositoryMock.Verify(x => x.GetConfiguration(), Times.Once);

            _embyClientMock.Verify(x => x.GetItemAsync(It.IsAny<ItemQuery>(), It.IsAny<Guid>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async void GetPersonByIdShouldGoNotGoToEmby()
        {
            returnedPerson = new Person
            {
                Id = basePerson.Id,
                Name = "name",
                Primary = "",
                MovieCount = 10,
                BirthDate = new DateTime(2000, 1, 1),
                ChildCount = 20,
                EpisodeCount = 10,
                Etag = "etag",
                HomePageUrl = "localhost.be",
                IMDB = "12345",
                TMDB = "12345",
                OverView = "Lots of text",
                SeriesCount = 1,
                SortName = "name",
                Synced = true
            };
            _personREpositoryMock.Setup(x => x.GetPersonById(It.IsAny<Guid>())).Returns(returnedPerson);

            var person = await _subject.GetPersonById(returnedPerson.Id);

            person.Should().NotBeNull();
            person.Id.Should().Be(basePerson.Id);
            person.Name.Should().Be(basePerson.Name);
            person.Primary.Should().Be(basePerson.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            person.MovieCount.Should().Be(basePerson.MovieCount);
            person.BirthDate.Should().Be(basePerson.PremiereDate);
            person.ChildCount.Should().Be(basePerson.ChildCount);
            person.EpisodeCount.Should().Be(basePerson.EpisodeCount);
            person.Etag.Should().Be(basePerson.Etag);
            //person.HomePageUrl.Should().Be(basePerson.HomePageUrl);
            person.IMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value);
            person.TMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value);
            person.OverView.Should().Be(basePerson.Overview);
            person.SeriesCount.Should().Be(basePerson.SeriesCount);
            person.SortName.Should().Be(basePerson.SortName);
            person.Synced.Should().BeTrue();

            _personREpositoryMock.Verify(x => x.GetPersonById(It.IsAny<Guid>()), Times.Once);
            _personREpositoryMock.Verify(x => x.AddOrUpdatePerson(It.IsAny<Person>()), Times.Never);

            _configurationRepositoryMock.Verify(x => x.GetConfiguration(), Times.Never);

            _embyClientMock.Verify(x => x.GetItemAsync(It.IsAny<ItemQuery>(), It.IsAny<Guid>(), CancellationToken.None), Times.Never);
        }
    }
}
