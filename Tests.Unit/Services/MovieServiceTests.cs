using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
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
                Id = string.Empty,
                Name = "The lord of the rings",
                PremiereDate = new DateTime(2002, 4, 2, 0, 0, 0),
                DateCreated = new DateTime(2018, 1, 1, 0, 0, 0),
                OfficialRating = "R",
                RunTimeTicks = 120000000000,
                Primary = "primaryImage",
                Genres = new List<string> { "id1" }
            };

            _movieTwo = new Movie
            {
                CommunityRating = (float)1.7,
                Id = string.Empty,
                Name = "The lord of the rings, two towers",
                PremiereDate = new DateTime(2002, 4, 2, 0, 0, 0),
                DateCreated = new DateTime(2017, 1, 1, 0, 0, 0),
                OfficialRating = "R",
                RunTimeTicks = 6000000000000,
                Primary = "primaryImage",
                Genres = new List<string> { "id2" }
            };

            var movieRepositoryMock = new Mock<IMovieRepository>();
            movieRepositoryMock.Setup(x => x.GetAll(It.IsAny<IEnumerable<string>>()))
                .Returns(new List<Movie> { _movieOne, _movieTwo });
            var collectionRepositoryMock = new Mock<ICollectionRepository>();
            collectionRepositoryMock.Setup(x => x.GetCollectionByTypes(It.IsAny<IEnumerable<CollectionType>>())).Returns(_collections);

            var genreRepositoryMock = new Mock<IGenreRepository>();
            var personServiceMock = new Mock<IPersonService>();
            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock.Setup(x => x.GetUserSettings())
                .Returns(new UserSettings { ToShortMovie = 10, MovieCollectionTypes = new List<CollectionType> { CollectionType.Movies } });
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            var taskRepositoryMock = new Mock<IJobRepository>();
            _subject = new MovieService(movieRepositoryMock.Object, collectionRepositoryMock.Object, genreRepositoryMock.Object,
                personServiceMock.Object, settingsServiceMock.Object, statisticsRepositoryMock.Object, taskRepositoryMock.Object);
        }

        [Fact]
        public void GetCollectionsFromDatabase()
        {
            var collections = _subject.GetMovieCollections().ToList();

            collections.Should().NotBeNull();
            collections.Count().Should().Be(2);
        }

        [Fact]
        public void GetMovieCountStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.MovieCount.Should().NotBeNull();
            stat.MovieCount.Title.Should().Be(Constants.Movies.TotalMovies);
            stat.MovieCount.Value.Should().Be(2);
        }

        [Fact]
        public void GetGenreCountStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.GenreCount.Should().NotBeNull();
            stat.GenreCount.Title.Should().Be(Constants.Movies.TotalGenres);
            stat.GenreCount.Value.Should().Be(2);
        }

        [Fact]
        public void GetLowestRatedStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.LowestRatedMovie.Should().NotBeNull();
            stat.LowestRatedMovie.Title.Should().Be(Constants.Movies.LowestRated);
            stat.LowestRatedMovie.Name.Should().Be(_movieOne.Name);
            stat.LowestRatedMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.LowestRatedMovie.DurationMinutes.Should().Be(200);
            stat.LowestRatedMovie.MediaId.Should().Be(_movieOne.Id);
            stat.LowestRatedMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.LowestRatedMovie.Tag.Should().Be(_movieOne.Primary);
            stat.LowestRatedMovie.Year.Should().Be(2002);
        }

        [Fact]
        public void GetHighestRatedStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.HighestRatedMovie.Should().NotBeNull();
            stat.HighestRatedMovie.Title.Should().Be(Constants.Movies.HighestRated);
            stat.HighestRatedMovie.Name.Should().Be(_movieOne.Name);
            stat.HighestRatedMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.HighestRatedMovie.DurationMinutes.Should().Be(200);
            stat.HighestRatedMovie.MediaId.Should().Be(_movieOne.Id);
            stat.HighestRatedMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.HighestRatedMovie.Tag.Should().Be(_movieOne.Primary);
            stat.HighestRatedMovie.Year.Should().Be(2002);
        }

        [Fact]
        public void GetOldestPremieredStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.OldestPremieredMovie.Should().NotBeNull();
            stat.OldestPremieredMovie.Title.Should().Be(Constants.Movies.OldestPremiered);
            stat.OldestPremieredMovie.Name.Should().Be(_movieOne.Name);
            stat.OldestPremieredMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.OldestPremieredMovie.DurationMinutes.Should().Be(200);
            stat.OldestPremieredMovie.MediaId.Should().Be(_movieOne.Id);
            stat.OldestPremieredMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.OldestPremieredMovie.Tag.Should().Be(_movieOne.Primary);
            stat.OldestPremieredMovie.Year.Should().Be(2002);
        }

        [Fact]
        public void GetYoungestPremieredStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.YoungestPremieredMovie.Should().NotBeNull();
            stat.YoungestPremieredMovie.Title.Should().Be(Constants.Movies.YoungestPremiered);
            stat.YoungestPremieredMovie.Name.Should().Be(_movieOne.Name);
            stat.YoungestPremieredMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.YoungestPremieredMovie.DurationMinutes.Should().Be(200);
            stat.YoungestPremieredMovie.MediaId.Should().Be(_movieOne.Id);
            stat.YoungestPremieredMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.YoungestPremieredMovie.Tag.Should().Be(_movieOne.Primary);
            stat.YoungestPremieredMovie.Year.Should().Be(2002);
        }

        [Fact]
        public void GetShortestStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.ShortestMovie.Should().NotBeNull();
            stat.ShortestMovie.Title.Should().Be(Constants.Movies.Shortest);
            stat.ShortestMovie.Name.Should().Be(_movieOne.Name);
            stat.ShortestMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.ShortestMovie.DurationMinutes.Should().Be(200);
            stat.ShortestMovie.MediaId.Should().Be(_movieOne.Id);
            stat.ShortestMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.ShortestMovie.Tag.Should().Be(_movieOne.Primary);
            stat.ShortestMovie.Year.Should().Be(2002);
        }

        [Fact]
        public void GetLongestStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.LongestMovie.Should().NotBeNull();
            stat.LongestMovie.Title.Should().Be(Constants.Movies.Longest);
            stat.LongestMovie.Name.Should().Be(_movieTwo.Name);
            stat.LongestMovie.CommunityRating.Should().Be(_movieTwo.CommunityRating.ToString());
            stat.LongestMovie.DurationMinutes.Should().Be(10000);
            stat.LongestMovie.MediaId.Should().Be(_movieTwo.Id);
            stat.LongestMovie.OfficialRating.Should().Be(_movieTwo.OfficialRating);
            stat.LongestMovie.Tag.Should().Be(_movieTwo.Primary);
            stat.LongestMovie.Year.Should().Be(2002);
        }

        [Fact]
        public void GetYoungestAddedStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.YoungestAddedMovie.Should().NotBeNull();
            stat.YoungestAddedMovie.Title.Should().Be(Constants.Movies.YoungestAdded);
            stat.YoungestAddedMovie.Name.Should().Be(_movieOne.Name);
            stat.YoungestAddedMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.YoungestAddedMovie.DurationMinutes.Should().Be(200);
            stat.YoungestAddedMovie.MediaId.Should().Be(_movieOne.Id);
            stat.YoungestAddedMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.YoungestAddedMovie.Tag.Should().Be(_movieOne.Primary);
            stat.YoungestAddedMovie.Year.Should().Be(2002);
        }

        [Fact]
        public void GetTotalPlayLengthStat()
        {
            var stat = _subject.GetMovieStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TotalPlayableTime.Should().NotBeNull();
            stat.TotalPlayableTime.Title.Should().Be(Constants.Movies.TotalPlayLength);
            stat.TotalPlayableTime.Days.Should().Be(7);
            stat.TotalPlayableTime.Hours.Should().Be(2);
            stat.TotalPlayableTime.Minutes.Should().Be(0);
        }
    }
}
