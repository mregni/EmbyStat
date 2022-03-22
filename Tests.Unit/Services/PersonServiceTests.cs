using System;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class PersonServiceTests
    {
        private readonly Person _basePerson;

        private Mock<IEmbyBaseHttpClient> EmbyClientMock { get; set; }
        private Mock<IPersonRepository> PersonRepositoryMock { get; set; }
        public PersonServiceTests()
        {
            _basePerson = new Person
            {
                Id = "1",
                Name = "name",
                Primary = "",
                BirthDate = new DateTime(2000, 1, 1),
                Etag = "etag",
                IMDB = "12345",
                TMDB = "12345",
                OverView = "Lots of text",
                SortName = "name"
            };
        }

        private PersonService CreatePersonService(Person person)
        {
            PersonRepositoryMock = new Mock<IPersonRepository>();
            PersonRepositoryMock.Setup(x => x.GetPersonByName(It.IsAny<string>())).Returns(person);

            EmbyClientMock = new Mock<IEmbyBaseHttpClient>();
            EmbyClientMock.Setup(x => x.GetPersonByName(It.IsAny<string>()))
                .Returns(_basePerson);

            var movieRepositoryMock = new Mock<IMovieRepository>();
            movieRepositoryMock.Setup(x => x.GetMediaCountForPerson(It.IsAny<string>())).Returns(10);
            var showRepositoryMock = new Mock<IShowRepository>();
            showRepositoryMock.Setup(x => x.GetMediaCountForPerson(It.IsAny<string>())).Returns(2);
            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(new UserSettings { MediaServer = new MediaServerSettings { Type = ServerType.Emby } });

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(EmbyClientMock.Object);

            return new PersonService(PersonRepositoryMock.Object, showRepositoryMock.Object, movieRepositoryMock.Object, strategy.Object, settingsServiceMock.Object);
        }

        [Fact]
        public void GetPersonByNameNotInDatabase()
        {
            var subject = CreatePersonService(null);
            var person = subject.GetPersonByNameForMovies("name");

            person.Should().NotBeNull();
            person.Id.Should().Be(_basePerson.Id);
            person.Name.Should().Be(_basePerson.Name);
            person.Primary.Should().Be(_basePerson.Primary);
            person.MovieCount.Should().Be(10);
            person.BirthDate.Should().Be(_basePerson.BirthDate);
            person.Etag.Should().Be(_basePerson.Etag);
            person.IMDB.Should().Be(_basePerson.IMDB);
            person.TMDB.Should().Be(_basePerson.TMDB);
            person.OverView.Should().Be(_basePerson.OverView);
            //TODO Re-enable show count
            //person.ShowCount.Should().Be(2);
            person.SortName.Should().Be(_basePerson.SortName);

            PersonRepositoryMock.Verify(x => x.GetPersonByName(It.IsAny<string>()), Times.Once);

            EmbyClientMock.Verify(x => x.GetPersonByName(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetPersonByNameWithEmbyFail()
        {
            PersonRepositoryMock = new Mock<IPersonRepository>();
            PersonRepositoryMock.Setup(x => x.GetPersonByName(It.IsAny<string>())).Returns((Person) null);

            EmbyClientMock = new Mock<IEmbyBaseHttpClient>();
            EmbyClientMock.Setup(x => x.GetPersonByName(It.IsAny<string>())).Throws(new Exception());

            var movieRepositoryMock = new Mock<IMovieRepository>();
            var showRepositoryMock = new Mock<IShowRepository>();
            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(new UserSettings { MediaServer = new MediaServerSettings { Type = ServerType.Emby }});

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(EmbyClientMock.Object);

            var subject = new PersonService(PersonRepositoryMock.Object, showRepositoryMock.Object, movieRepositoryMock.Object, strategy.Object, settingsServiceMock.Object);
            var person = subject.GetPersonByNameForMovies("testing name");

            person.Should().BeNull();
        }

        [Fact]
        public void GetPersonByNameInDatabase()
        {
            var databasePerson = new Person
            {
                Id = _basePerson.Id,
                Name = "name",
                Primary = "",
                BirthDate = new DateTime(2000, 1, 1),
                Etag = "etag",
                HomePageUrl = "localhost.be",
                IMDB = "12345",
                TMDB = "12345",
                OverView = "Lots of text",
                SortName = "name"
            };

            var subject = CreatePersonService(databasePerson);
            var person = subject.GetPersonByNameForMovies(databasePerson.Id);

            person.Should().NotBeNull();
            person.Id.Should().Be(databasePerson.Id);
            person.Name.Should().Be(databasePerson.Name);
            person.Primary.Should().Be(databasePerson.Primary);
            person.MovieCount.Should().Be(databasePerson.MovieCount);
            person.BirthDate.Should().Be(databasePerson.BirthDate);
            person.Etag.Should().Be(databasePerson.Etag);
            person.IMDB.Should().Be(databasePerson.IMDB);
            person.TMDB.Should().Be(databasePerson.TMDB);
            person.OverView.Should().Be(databasePerson.OverView);
            person.ShowCount.Should().Be(databasePerson.ShowCount);
            person.SortName.Should().Be(databasePerson.SortName);

            PersonRepositoryMock.Verify(x => x.GetPersonByName(It.IsAny<string>()), Times.Once);

            EmbyClientMock.Verify(x => x.GetPersonByName(It.IsAny<string>()), Times.Never);
        }
    }
}
