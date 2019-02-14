using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.EmbyClient;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
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
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly Mock<ISettingsService> _settingsServiceMock;
        private readonly Mock<IEmbyClient> _embyClientMock;
        private Person returnedPerson;
        private BaseItemDto basePerson;
        public PersonServiceTests()
        {
            basePerson = new BaseItemDto
            {
                Id = string.Empty,
                Name = "name",
                ImageTags = new Dictionary<ImageType, string> { { ImageType.Primary, "" } },
                MovieCount = 10,
                PremiereDate = new DateTime(2000, 1, 1),
                Etag = "etag",
                ProviderIds = new Dictionary<string, string> { { "Imdb", "12345" }, { "Tmdb", "12345" } },
                Overview = "Lots of text",
                SeriesCount = 1,
                SortName = "name"
            };

            var embySettings = new EmbySettings
            {
                AccessToken = "12345",
                ServerAddress = "localhost",
                ServerPort = 8096,
                ServerProtocol = ConnectionProtocol.Http
            };

            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock.Setup(x => x.AddOrUpdatePerson(It.IsAny<Person>()));

            _settingsServiceMock = new Mock<ISettingsService>();
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(new UserSettings { Emby = embySettings });

            _embyClientMock = new Mock<IEmbyClient>();
            _embyClientMock.Setup(x => x.GetPersonByNameAsync(It.IsAny<string>(), CancellationToken.None))
                .Returns(Task.FromResult(basePerson));

            _subject = new PersonService(_personRepositoryMock.Object, _settingsServiceMock.Object, _embyClientMock.Object);
        }

        [Fact]
        public async void GetPersonByIdNotSyncedPerson()
        {
            returnedPerson = new Person
            {
                Id = string.Empty,
                Name = "name",
                Synced = false
            };
            _personRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<string>())).Returns(returnedPerson);

            var person = await _subject.GetPersonById(string.Empty);

            person.Should().NotBeNull();
            person.Id.Should().Be(basePerson.Id);
            person.Name.Should().Be(basePerson.Name);
            person.Primary.Should().Be(basePerson.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            person.MovieCount.Should().Be(basePerson.MovieCount);
            person.BirthDate.Should().Be(basePerson.PremiereDate);
            person.Etag.Should().Be(basePerson.Etag);
            person.IMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value);
            person.TMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value);
            person.OverView.Should().Be(basePerson.Overview);
            person.SeriesCount.Should().Be(basePerson.SeriesCount);
            person.SortName.Should().Be(basePerson.SortName);
            person.Synced.Should().BeTrue();

            _personRepositoryMock.Verify(x => x.GetPersonById(It.IsAny<string>()), Times.Once);
            _personRepositoryMock.Verify(x => x.AddOrUpdatePerson(It.IsAny<Person>()), Times.Once);

            _settingsServiceMock.Verify(x => x.GetUserSettings(), Times.Once);

            _embyClientMock.Verify(x => x.GetPersonByNameAsync(It.IsAny<string>(), CancellationToken.None), Times.Once);
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
                seriesCount = 10,
                Etag = "etag",
                HomePageUrl = "localhost.be",
                IMDB = "12345",
                TMDB = "12345",
                OverView = "Lots of text",
                SeriesCount = 1,
                SortName = "name",
                Synced = true
            };
            _personRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<string>())).Returns(returnedPerson);

            var person = await _subject.GetPersonById(returnedPerson.Id);

            person.Should().NotBeNull();
            person.Id.Should().Be(basePerson.Id);
            person.Name.Should().Be(basePerson.Name);
            person.Primary.Should().Be(basePerson.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            person.MovieCount.Should().Be(basePerson.MovieCount);
            person.BirthDate.Should().Be(basePerson.PremiereDate);
            person.Etag.Should().Be(basePerson.Etag);
            person.IMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value);
            person.TMDB.Should().Be(basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value);
            person.OverView.Should().Be(basePerson.Overview);
            person.SeriesCount.Should().Be(basePerson.SeriesCount);
            person.SortName.Should().Be(basePerson.SortName);
            person.Synced.Should().BeTrue();

            _personRepositoryMock.Verify(x => x.GetPersonById(It.IsAny<string>()), Times.Once);
            _personRepositoryMock.Verify(x => x.AddOrUpdatePerson(It.IsAny<Person>()), Times.Never);

            _settingsServiceMock.Verify(x => x.GetUserSettings(), Times.Never);

            _embyClientMock.Verify(x => x.GetPersonByNameAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
        }
    }
}
