using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class PersonServiceTests
    {
        private readonly BaseItemDto _basePerson;

        private Mock<IEmbyClient> EmbyClientMock { get; set; }
        private Mock<IPersonRepository> PersonRepositoryMock { get; set; }
        public PersonServiceTests()
        {
            _basePerson = new BaseItemDto
            {
                Id = "1",
                Name = "name",
                ImageTags = new Dictionary<ImageType, string> { { ImageType.Primary, "" } },
                PremiereDate = new DateTime(2000, 1, 1),
                Etag = "etag",
                ProviderIds = new Dictionary<string, string> { { "Imdb", "12345" }, { "Tmdb", "12345" } },
                Overview = "Lots of text",
                SortName = "name"
            };
        }

        private PersonService CreatePersonService(Person person)
        {
            PersonRepositoryMock = new Mock<IPersonRepository>();
            PersonRepositoryMock.Setup(x => x.GetPersonByName(It.IsAny<string>())).Returns(person);

            EmbyClientMock = new Mock<IEmbyClient>();
            EmbyClientMock.Setup(x => x.GetPersonByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_basePerson);

            var movieRepositoryMock = new Mock<IMovieRepository>();
            movieRepositoryMock.Setup(x => x.GetMediaCountForPerson(It.IsAny<string>())).Returns(10);
            var showRepositoryMock = new Mock<IShowRepository>();
            showRepositoryMock.Setup(x => x.GetMediaCountForPerson(It.IsAny<string>())).Returns(2);

            return new PersonService(PersonRepositoryMock.Object, showRepositoryMock.Object, movieRepositoryMock.Object, EmbyClientMock.Object);
        }

        [Fact]
        public async Task GetPersonByNameNotInDatabase()
        {
            var subject = CreatePersonService(null);
            var person = await subject.GetPersonByNameAsync("name");

            person.Should().NotBeNull();
            person.Id.Should().Be(_basePerson.Id);
            person.Name.Should().Be(_basePerson.Name);
            person.Primary.Should().Be(_basePerson.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            person.MovieCount.Should().Be(10);
            person.BirthDate.Should().Be(_basePerson.PremiereDate);
            person.Etag.Should().Be(_basePerson.Etag);
            person.IMDB.Should().Be(_basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value);
            person.TMDB.Should().Be(_basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value);
            person.OverView.Should().Be(_basePerson.Overview);
            person.ShowCount.Should().Be(2);
            person.SortName.Should().Be(_basePerson.SortName);

            PersonRepositoryMock.Verify(x => x.GetPersonByName(It.IsAny<string>()), Times.Once);

            EmbyClientMock.Verify(x => x.GetPersonByNameAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetPersonByNameWithEmbyFail()
        {
            PersonRepositoryMock = new Mock<IPersonRepository>();
            PersonRepositoryMock.Setup(x => x.GetPersonByName(It.IsAny<string>())).Returns((Person) null);

            EmbyClientMock = new Mock<IEmbyClient>();
            EmbyClientMock.Setup(x => x.GetPersonByNameAsync(It.IsAny<string>())).Throws(new Exception());

            var movieRepositoryMock = new Mock<IMovieRepository>();
            var showRepositoryMock = new Mock<IShowRepository>();

            var subject = new PersonService(PersonRepositoryMock.Object, showRepositoryMock.Object, movieRepositoryMock.Object, EmbyClientMock.Object);
            var person = await subject.GetPersonByNameAsync("testing name");

            person.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByNameInDatabase()
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
            var person = await subject.GetPersonByNameAsync(databasePerson.Id);

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

            EmbyClientMock.Verify(x => x.GetPersonByNameAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
