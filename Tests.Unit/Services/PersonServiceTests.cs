using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    [Collection("Services collection")]
    public class PersonServiceTests
    {
        private readonly PersonService _subject;
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly Mock<IEmbyClient> _embyClientMock;
        private Person _returnedPerson;
        private readonly BaseItemDto _basePerson;
        public PersonServiceTests()
        {
            _basePerson = new BaseItemDto
            {
                Id = string.Empty,
                Name = "name",
                ImageTags = new Dictionary<ImageType, string> { { ImageType.Primary, "" } },
                MovieCount = 10,
                PremiereDate = new DateTime(2000, 1, 1),
                Etag = "etag",
                ProviderIds = new Dictionary<string, string> { { "Imdb", "12345" }, { "Tmdb", "12345" } },
                Overview = "Lots of text",
                SeriesCount = 2,
                SortName = "name"
            };

            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock.Setup(x => x.AddOrUpdatePerson(It.IsAny<Person>()));

            _embyClientMock = new Mock<IEmbyClient>();
            _embyClientMock.Setup(x => x.GetPersonByNameAsync(It.IsAny<string>(), CancellationToken.None))
                .Returns(Task.FromResult(_basePerson));

            var movieRepositoryMock = new Mock<IMovieRepository>();
            movieRepositoryMock.Setup(x => x.GetMovieCountForPerson(It.IsAny<string>())).Returns(10);
            var showRepositoryMock = new Mock<IShowRepository>();
            showRepositoryMock.Setup(x => x.GetShowCountForPerson(It.IsAny<string>())).Returns(2);

            _subject = new PersonService(_personRepositoryMock.Object,  showRepositoryMock.Object, movieRepositoryMock.Object, _embyClientMock.Object);
        }

        [Fact]
        public async void GetPersonByIdNotSyncedPerson()
        {
            _returnedPerson = new Person
            {
                Id = string.Empty,
                Name = "name",
                Synced = false
            };
            _personRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<string>())).Returns(_returnedPerson);

            var person = await _subject.GetPersonByIdAsync(string.Empty);

            person.Should().NotBeNull();
            person.Id.Should().Be(_basePerson.Id);
            person.Name.Should().Be(_basePerson.Name);
            person.Primary.Should().Be(_basePerson.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value);
            person.MovieCount.Should().Be(_basePerson.MovieCount);
            person.BirthDate.Should().Be(_basePerson.PremiereDate);
            person.Etag.Should().Be(_basePerson.Etag);
            person.IMDB.Should().Be(_basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value);
            person.TMDB.Should().Be(_basePerson.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value);
            person.OverView.Should().Be(_basePerson.Overview);
            person.ShowCount.Should().Be(_basePerson.SeriesCount);
            person.SortName.Should().Be(_basePerson.SortName);
            person.Synced.Should().BeTrue();

            _personRepositoryMock.Verify(x => x.GetPersonById(It.IsAny<string>()), Times.Once);
            _personRepositoryMock.Verify(x => x.AddOrUpdatePerson(It.IsAny<Person>()), Times.Once);
            
            _embyClientMock.Verify(x => x.GetPersonByNameAsync(It.IsAny<string>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async void GetPersonByIdShouldNotGoToEmby()
        {
            _returnedPerson = new Person
            {
                Id = _basePerson.Id,
                Name = "name",
                Primary = "",
                MovieCount = 10,
                BirthDate = new DateTime(2000, 1, 1),
                Etag = "etag",
                HomePageUrl = "localhost.be",
                IMDB = "12345",
                TMDB = "12345",
                OverView = "Lots of text",
                ShowCount = 10,
                SortName = "name",
                Synced = true
            };
            _personRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<string>())).Returns(_returnedPerson);

            var person = await _subject.GetPersonByIdAsync(_returnedPerson.Id);

            person.Should().NotBeNull();
            person.Id.Should().Be(_returnedPerson.Id);
            person.Name.Should().Be(_returnedPerson.Name);
            person.Primary.Should().Be(_returnedPerson.Primary);
            person.MovieCount.Should().Be(_returnedPerson.MovieCount);
            person.BirthDate.Should().Be(_returnedPerson.BirthDate);
            person.Etag.Should().Be(_returnedPerson.Etag);
            person.IMDB.Should().Be(_returnedPerson.IMDB);
            person.TMDB.Should().Be(_returnedPerson.TMDB);
            person.OverView.Should().Be(_returnedPerson.OverView);
            person.ShowCount.Should().Be(_returnedPerson.ShowCount);
            person.SortName.Should().Be(_returnedPerson.SortName);
            person.Synced.Should().BeTrue();

            _personRepositoryMock.Verify(x => x.GetPersonById(It.IsAny<string>()), Times.Once);
            _personRepositoryMock.Verify(x => x.AddOrUpdatePerson(It.IsAny<Person>()), Times.Never);

            _embyClientMock.Verify(x => x.GetPersonByNameAsync(It.IsAny<string>(), CancellationToken.None), Times.Never);
        }
    }
}
