using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;
using Constants = EmbyStat.Common.Constants;

namespace Tests.Unit.Services
{
    [Collection("Services collection")]
    public class MovieServiceTests
    {
        private readonly MovieService _subject;
        private readonly List<Collection> _collections;
        private readonly Movie _movieOne;
        private readonly Movie _movieTwo;

        public MovieServiceTests()
        {
            _collections = new List<Collection>
            {
                new Collection{ Id = string.Empty, Name = "collection1", PrimaryImage = "image1", Type = CollectionType.Movies},
                new Collection{ Id = string.Empty, Name = "collection2", PrimaryImage = "image2", Type = CollectionType.Movies}
            };

            _movieOne = new Movie
            {
                CommunityRating = (float)1.7,
                Id = 0,
                Name = "The lord of the rings",
                PremiereDate = new DateTime(2002, 4, 2, 0, 0, 0),
                DateCreated = new DateTime(2018, 1, 1, 0, 0, 0),
                OfficialRating = "R",
                RunTimeTicks = 120000000000,
                Primary = "primaryImage",
                Genres = new List<string> { "id1" },
                People = new List<ExtraPerson> {  new ExtraPerson { Id = Guid.NewGuid().ToString(), Name = "Batman", Type = "Actor"} }
            };

            _movieTwo = new Movie
            {
                CommunityRating = (float)1.7,
                Id = 0,
                Name = "The lord of the rings, two towers",
                PremiereDate = new DateTime(2002, 4, 2, 0, 0, 0),
                DateCreated = new DateTime(2017, 1, 1, 0, 0, 0),
                OfficialRating = "R",
                RunTimeTicks = 6000000000000,
                Primary = "primaryImage",
                Genres = new List<string> { "id2" },
                People = new List<ExtraPerson> { new ExtraPerson { Id = Guid.NewGuid().ToString(), Name = "Batman", Type = "Actor" } }
            };

            var movieRepositoryMock = new Mock<IMovieRepository>();
            movieRepositoryMock.Setup(x => x.GetAll(It.IsAny<IEnumerable<string>>()))
                .Returns(new List<Movie> { _movieOne, _movieTwo });
            var collectionRepositoryMock = new Mock<ICollectionRepository>();
            collectionRepositoryMock.Setup(x => x.GetCollectionByTypes(It.IsAny<IEnumerable<CollectionType>>())).Returns(_collections);

            var personServiceMock = new Mock<IPersonService>();
            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock.Setup(x => x.GetUserSettings())
                .Returns(new UserSettings { ToShortMovie = 10, MovieCollectionTypes = new List<CollectionType> { CollectionType.Movies } });
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            var taskRepositoryMock = new Mock<IJobRepository>();
            _subject = new MovieService(movieRepositoryMock.Object, collectionRepositoryMock.Object, personServiceMock.Object, settingsServiceMock.Object, statisticsRepositoryMock.Object, taskRepositoryMock.Object);
        }

        [Fact]
        public void GetCollectionsFromDatabase()
        {
            var collections = _subject.GetMovieCollections().ToList();

            collections.Should().NotBeNull();
            collections.Count.Should().Be(2);
        }

        [Fact]
        public async void GetMovieCountStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.MovieCount.Should().NotBeNull();
            stat.General.MovieCount.Title.Should().Be(Constants.Movies.TotalMovies);
            stat.General.MovieCount.Value.Should().Be(2);
        }

        [Fact]
        public async void GetGenreCountStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.GenreCount.Should().NotBeNull();
            stat.General.GenreCount.Title.Should().Be(Constants.Movies.TotalGenres);
            stat.General.GenreCount.Value.Should().Be(2);
        }

        [Fact]
        public async void GetLowestRatedStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.LowestRatedMovie.Should().NotBeNull();
            stat.General.LowestRatedMovie.Title.Should().Be(Constants.Movies.LowestRated);
            stat.General.LowestRatedMovie.Name.Should().Be(_movieOne.Name);
            stat.General.LowestRatedMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.LowestRatedMovie.DurationMinutes.Should().Be(200);
            stat.General.LowestRatedMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.LowestRatedMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.LowestRatedMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.LowestRatedMovie.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetHighestRatedStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.HighestRatedMovie.Should().NotBeNull();
            stat.General.HighestRatedMovie.Title.Should().Be(Constants.Movies.HighestRated);
            stat.General.HighestRatedMovie.Name.Should().Be(_movieOne.Name);
            stat.General.HighestRatedMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.HighestRatedMovie.DurationMinutes.Should().Be(200);
            stat.General.HighestRatedMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.HighestRatedMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.HighestRatedMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.HighestRatedMovie.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetOldestPremieredStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.OldestPremieredMovie.Should().NotBeNull();
            stat.General.OldestPremieredMovie.Title.Should().Be(Constants.Movies.OldestPremiered);
            stat.General.OldestPremieredMovie.Name.Should().Be(_movieOne.Name);
            stat.General.OldestPremieredMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.OldestPremieredMovie.DurationMinutes.Should().Be(200);
            stat.General.OldestPremieredMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.OldestPremieredMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.OldestPremieredMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.OldestPremieredMovie.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetYoungestPremieredStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.YoungestPremieredMovie.Should().NotBeNull();
            stat.General.YoungestPremieredMovie.Title.Should().Be(Constants.Movies.YoungestPremiered);
            stat.General.YoungestPremieredMovie.Name.Should().Be(_movieOne.Name);
            stat.General.YoungestPremieredMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.YoungestPremieredMovie.DurationMinutes.Should().Be(200);
            stat.General.YoungestPremieredMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.YoungestPremieredMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.YoungestPremieredMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.YoungestPremieredMovie.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetShortestStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.ShortestMovie.Should().NotBeNull();
            stat.General.ShortestMovie.Title.Should().Be(Constants.Movies.Shortest);
            stat.General.ShortestMovie.Name.Should().Be(_movieOne.Name);
            stat.General.ShortestMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.ShortestMovie.DurationMinutes.Should().Be(200);
            stat.General.ShortestMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.ShortestMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.ShortestMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.ShortestMovie.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetLongestStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.LongestMovie.Should().NotBeNull();
            stat.General.LongestMovie.Title.Should().Be(Constants.Movies.Longest);
            stat.General.LongestMovie.Name.Should().Be(_movieTwo.Name);
            stat.General.LongestMovie.CommunityRating.Should().Be(_movieTwo.CommunityRating.ToString());
            stat.General.LongestMovie.DurationMinutes.Should().Be(10000);
            stat.General.LongestMovie.MediaId.Should().Be(_movieTwo.Id);
            stat.General.LongestMovie.OfficialRating.Should().Be(_movieTwo.OfficialRating);
            stat.General.LongestMovie.Tag.Should().Be(_movieTwo.Primary);
            stat.General.LongestMovie.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetYoungestAddedStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.YoungestAddedMovie.Should().NotBeNull();
            stat.General.YoungestAddedMovie.Title.Should().Be(Constants.Movies.YoungestAdded);
            stat.General.YoungestAddedMovie.Name.Should().Be(_movieOne.Name);
            stat.General.YoungestAddedMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.YoungestAddedMovie.DurationMinutes.Should().Be(200);
            stat.General.YoungestAddedMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.YoungestAddedMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.YoungestAddedMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.YoungestAddedMovie.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetTotalPlayLengthStat()
        {
            var stat = await _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.TotalPlayableTime.Should().NotBeNull();
            stat.General.TotalPlayableTime.Title.Should().Be(Constants.Movies.TotalPlayLength);
            stat.General.TotalPlayableTime.Days.Should().Be(7);
            stat.General.TotalPlayableTime.Hours.Should().Be(2);
            stat.General.TotalPlayableTime.Minutes.Should().Be(0);
        }
    }
}
