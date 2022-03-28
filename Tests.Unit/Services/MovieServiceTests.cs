using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Cards;
using FluentAssertions;
using Moq;
using Tests.Unit.Builders;
using Xunit;
using Constants = EmbyStat.Common.Constants;

namespace Tests.Unit.Services
{
    public class MovieServiceTests
    {
        private readonly MovieService _subject;
        private readonly List<Library> _collections;
        private readonly Movie _movieOne;
        private readonly Movie _movieTwo;
        private readonly Movie _movieThree;
        private readonly Mock<ISettingsService> _settingsServiceMock;

        public MovieServiceTests()
        {
            _collections = new List<Library>
            {
                new Library{ Id = string.Empty, Name = "collection1", Primary = "image1", Type = LibraryType.Movies},
                new Library{ Id = string.Empty, Name = "collection2", Primary = "image2", Type = LibraryType.Movies}
            };

            var actorIdOne = Guid.NewGuid();

            _movieOne = new MovieBuilder(Guid.NewGuid().ToString())
                .AddCommunityRating((float)1.7)
                .AddOfficialRating("R")
                .AddPremiereDate(new DateTime(2002, 4, 2, 0, 0, 0))
                .AddRunTimeTicks(2, 10, 0)
                .AddName("The lord of the rings")
                .AddSortName("The-lord-of-the-rings")
                .AddGenres("Action", "Drama")
                .Build();

            _movieTwo = new MovieBuilder(Guid.NewGuid().ToString())
                .AddCommunityRating((float)2.8)
                .AddOfficialRating("R")
                .AddPremiereDate(new DateTime(2003, 4, 2, 0, 0, 0))
                .AddRunTimeTicks(3, 30, 0)
                .AddName("The lord of the rings, two towers")
                .AddSortName("The-lord-of-the-rings,-two-towers")
                .AddPerson(new ExtraPerson { Type = PersonType.Director, Name = "Frodo", Id = Guid.NewGuid().ToString() })
                .AddPerson(new ExtraPerson { Type = PersonType.Actor, Name = "Frodo", Id = actorIdOne.ToString() })
                .AddGenres("Action", "Comedy")
                .AddImdb("0002")
                .Build();

            _movieThree = new MovieBuilder(Guid.NewGuid().ToString())
                .AddCommunityRating((float)3.2)
                .AddOfficialRating("B")
                .AddPremiereDate(new DateTime(2004, 4, 2, 0, 0, 0))
                .AddRunTimeTicks(3, 50, 0)
                .AddName("The lord of the rings, return of the king")
                .AddSortName("The-lord-of-the-rings,-return-of-the-king")
                .AddGenres("Comedy")
                .AddPerson(new ExtraPerson { Type = PersonType.Actor, Name = "Frodo", Id = actorIdOne.ToString() })
                .AddPerson(new ExtraPerson { Type = PersonType.Director, Name = "Frodo", Id = Guid.NewGuid().ToString() })
                .AddPerson(new ExtraPerson { Type = PersonType.Writer, Name = "Frodo", Id = Guid.NewGuid().ToString() })
                .AddImdb("0003")
                .Build();

            _settingsServiceMock = new Mock<ISettingsService>();
            _settingsServiceMock
                .Setup(x => x.GetUserSettings())
                .Returns(new UserSettings { ToShortMovie = 10, MovieLibraries = new List<LibraryContainer> { new() { Id = _collections[0].Id }, new() { Id = _collections[1].Id } }, ToShortMovieEnabled = true });
            _subject = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree);
        }

        private MovieService CreateMovieService(Mock<ISettingsService> settingsServiceMock, params Movie[] movies)
        {
            var movieRepositoryMock = new Mock<IMovieRepository>();
            movieRepositoryMock
                .Setup(x => x.GetAll(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.ToList());
            movieRepositoryMock
                .Setup(x => x.GetAllWithImdbId(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.ToList());
            movieRepositoryMock
                .Setup(x => x.GetToShortMovieList(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>()))
                .Returns(movies.ToList());
            movieRepositoryMock
                .Setup(x => x.GetMoviesWithoutImdbId(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.ToList());
            movieRepositoryMock
                .Setup(x => x.GetMoviesWithoutPrimaryImage(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.ToList());

            movieRepositoryMock
                .Setup(x => x.GetGenreCount(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.SelectMany(x => x.Genres).Distinct().Count);
            movieRepositoryMock
                .Setup(x => x.GetHighestRatedMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(movies.OrderByDescending(x => x.CommunityRating));
            movieRepositoryMock
                .Setup(x => x.GetLowestRatedMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(movies.Where(x => x.CommunityRating != null).OrderBy(x => x.CommunityRating));
            movieRepositoryMock
                .Setup(x => x.GetLatestAddedMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(movies.OrderByDescending(x => x.DateCreated));
            movieRepositoryMock
                .Setup(x => x.GetMediaCount(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.Length);
            movieRepositoryMock
                .Setup(x => x.GetNewestPremieredMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(movies.OrderByDescending(x => x.PremiereDate));
            movieRepositoryMock
                .Setup(x => x.GetOldestPremieredMedia(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(movies.OrderBy(x => x.PremiereDate));
            movieRepositoryMock
                .Setup(x => x.GetLongestMovie(It.IsAny<IReadOnlyList<string>>(), 5))
                .Returns(movies.OrderByDescending(x => x.RunTimeTicks));
            movieRepositoryMock
                .Setup(x => x.GetShortestMovie(It.IsAny<IReadOnlyList<string>>(), It.IsAny<long>(), 5))
                .Returns(movies.OrderBy(x => x.RunTimeTicks));
            movieRepositoryMock
                .Setup(x => x.GetTotalDiskSpace(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.Sum(x => x.MediaSources.FirstOrDefault()?.SizeInMb ?? 0));
            movieRepositoryMock
                .Setup(x => x.GetTotalRuntime(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.Sum(x => x.RunTimeTicks ?? 0));
            movieRepositoryMock
                .Setup(x => x.GetMoviesWithoutImdbId(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.Where(x => string.IsNullOrEmpty(x.IMDB)).ToList);
            movieRepositoryMock
                .Setup(x => x.GetMoviesWithoutPrimaryImage(It.IsAny<IReadOnlyList<string>>()))
                .Returns(movies.Where(x => string.IsNullOrEmpty(x.Primary)).ToList);
            movieRepositoryMock
                .Setup(x => x.GetToShortMovieList(It.IsAny<IReadOnlyList<string>>(), 10))
                .Returns(movies.Where(x => x.RunTimeTicks < new TimeSpan(0, 0, 10, 0).Ticks).ToList);
            movieRepositoryMock
                .Setup(x => x.GetPeopleCount(It.IsAny<IReadOnlyList<string>>(), PersonType.Actor))
                .Returns(movies.SelectMany(x => x.People).DistinctBy(x => x.Id).Count(x => x.Type == PersonType.Actor));
            movieRepositoryMock
                .Setup(x => x.GetPeopleCount(It.IsAny<IReadOnlyList<string>>(), PersonType.Writer))
                .Returns(movies.SelectMany(x => x.People).DistinctBy(x => x.Id).Count(x => x.Type == PersonType.Writer));
            movieRepositoryMock
                .Setup(x => x.GetPeopleCount(It.IsAny<IReadOnlyList<string>>(), PersonType.Director))
                .Returns(movies.SelectMany(x => x.People).DistinctBy(x => x.Id).Count(x => x.Type == PersonType.Director));

            var collectionRepositoryMock = new Mock<ILibraryRepository>();
            collectionRepositoryMock.Setup(x => x.GetLibrariesById(It.IsAny<IEnumerable<string>>())).Returns(_collections);

            var personServiceMock = new Mock<IPersonService>();
            foreach (var person in movies.SelectMany(x => x.People))
            {
                personServiceMock.Setup(x => x.GetPersonByNameForMovies(person.Name, It.IsAny<string>())).Returns(
                    new Person
                    {
                        Id = person.Id,
                        Name = person.Name,
                        BirthDate = new DateTime(2000, 1, 1),
                        Primary = "primary.jpg",
                        MovieCount = 0,
                        ShowCount = 0
                    });
            }

            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            var jobRepositoryMock = new Mock<IJobRepository>();
            return new MovieService(movieRepositoryMock.Object, collectionRepositoryMock.Object, personServiceMock.Object, settingsServiceMock.Object, statisticsRepositoryMock.Object, jobRepositoryMock.Object);
        }

        #region General

        [Fact]
        public void GetCollectionsFromDatabase()
        {
            var collections = _subject.GetMovieLibraries().ToList();

            collections.Should().NotBeNull();
            collections.Count.Should().Be(2);
        }

        [Fact]
        public void GetMovieCountStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Cards.Should().NotBeNull();
            stat.Cards.Count(x => x.Title == Constants.Movies.TotalMovies).Should().Be(1);

            var card = stat.Cards.First(x => x.Title == Constants.Movies.TotalMovies);
            card.Title.Should().Be(Constants.Movies.TotalMovies);
            card.Value.Should().Be("3");
        }

        [Fact]
        public void GetGenreCountStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Cards.Should().NotBeNull();
            stat.Cards.Count(x => x.Title == Constants.Common.TotalGenres).Should().Be(1);

            var card = stat.Cards.First(x => x.Title == Constants.Common.TotalGenres);
            card.Title.Should().Be(Constants.Common.TotalGenres);
            card.Value.Should().Be("3");
        }

        [Fact]
        public void GetLowestRatedStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Movies.LowestRated).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Movies.LowestRated);
            card.Title.Should().Be(Constants.Movies.LowestRated);
            card.Unit.Should().Be("/10");
            card.Values[0].Value.Should().Be(_movieOne.CommunityRating.ToString());
            card.Values[0].Label.Should().Be(_movieOne.Name);
            card.UnitNeedsTranslation.Should().Be(false);
            card.ValueType.Should().Be(ValueTypeEnum.None);
        }

        [Fact]
        public void GetHighestRatedStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Movies.HighestRated).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Movies.HighestRated);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Movies.HighestRated);
            card.Unit.Should().Be("/10");
            card.Values[0].Value.Should().Be(_movieThree.CommunityRating.ToString());
            card.Values[0].Label.Should().Be(_movieThree.Name);
            card.UnitNeedsTranslation.Should().Be(false);
            card.ValueType.Should().Be(ValueTypeEnum.None);
        }

        [Fact]
        public void GetOldestPremieredStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Movies.OldestPremiered).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Movies.OldestPremiered);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Movies.OldestPremiered);
            card.Unit.Should().Be("COMMON.DATE");
            card.Values[0].Value.Should().Be(_movieOne.PremiereDate?.ToString("s"));
            card.Values[0].Label.Should().Be(_movieOne.Name);
            card.UnitNeedsTranslation.Should().Be(true);
            card.ValueType.Should().Be(ValueTypeEnum.Date);
        }

        [Fact]
        public void GetNewestPremieredStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Movies.NewestPremiered).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Movies.NewestPremiered);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Movies.NewestPremiered);
            card.Unit.Should().Be("COMMON.DATE");
            card.Values[0].Value.Should().Be(_movieThree.PremiereDate?.ToString("s"));
            card.Values[0].Label.Should().Be(_movieThree.Name);
            card.UnitNeedsTranslation.Should().Be(true);
            card.ValueType.Should().Be(ValueTypeEnum.Date);
        }

        [Fact]
        public void GetShortestStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Movies.Shortest).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Movies.Shortest);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Movies.Shortest);
            card.Unit.Should().Be("COMMON.MIN");
            card.Values[0].Value.Should().Be(_movieOne.RunTimeTicks.ToString());
            card.Values[0].Label.Should().Be(_movieOne.Name);
            card.UnitNeedsTranslation.Should().Be(true);
            card.ValueType.Should().Be(ValueTypeEnum.Ticks);
        }

        [Fact]
        public void GetLongestStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Movies.Longest).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Movies.Longest);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Movies.Longest);
            card.Unit.Should().Be("COMMON.MIN");
            card.Values[0].Value.Should().Be(_movieThree.RunTimeTicks.ToString());
            card.Values[0].Label.Should().Be(_movieThree.Name);
            card.UnitNeedsTranslation.Should().Be(true);
            card.ValueType.Should().Be(ValueTypeEnum.Ticks);
        }

        [Fact]
        public void GetLatestAddedStat()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.TopCards.Count(x => x.Title == Constants.Movies.LatestAdded).Should().Be(1);

            var card = stat.TopCards.First(x => x.Title == Constants.Movies.LatestAdded);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Movies.LatestAdded);
            card.Unit.Should().Be("COMMON.DATE");
            card.Values[0].Value.Should().Be(_movieOne.DateCreated?.ToString("s"));
            card.Values[0].Label.Should().Be(_movieOne.Name);
            card.UnitNeedsTranslation.Should().Be(true);
            card.ValueType.Should().Be(ValueTypeEnum.Date);
        }

        [Fact]
        public void GetTotalDiskSize()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Cards.Count(x => x.Title == Constants.Common.TotalDiskSpace).Should().Be(1);

            var card = stat.Cards.First(x => x.Title == Constants.Common.TotalDiskSpace);
            card.Should().NotBeNull();
            card.Title.Should().Be(Constants.Common.TotalDiskSpace);
            card.Value.Should().Be("6000");
        }

        #endregion

        #region Charts

        [Fact]
        public void GetTotalPlayLengthStat()
        {
            var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(56, 34, 1).Build();
            var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = service.GetStatistics(_collections.Select(x => x.Id).ToList());
            stat.Cards.Count(x => x.Title == Constants.Movies.TotalPlayLength).Should().Be(1);

            var card = stat.Cards.First(x => x.Title == Constants.Movies.TotalPlayLength);
            card.Title.Should().Be(Constants.Movies.TotalPlayLength);
            card.Value.Should().Be("2|18|4");
        }

        [Fact]
        public void CalculateGenreChart()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Count.Should().Be(4);
            stat.Charts.Any(x => x.Title == Constants.CountPerGenre).Should().BeTrue();

            var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerGenre);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            graph.DataSets.Should().Be("[{\"Label\":\"Action\",\"Val0\":2},{\"Label\":\"Comedy\",\"Val0\":2},{\"Label\":\"Drama\",\"Val0\":1}]");
        }

        [Fact]
        public void CalculateOfficialRatingChart()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Count.Should().Be(4);
            stat.Charts.Any(x => x.Title == Constants.CountPerOfficialRating).Should().BeTrue();

            var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerOfficialRating);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            graph.DataSets.Should().Be("[{\"Label\":\"B\",\"Val0\":1},{\"Label\":\"R\",\"Val0\":2}]");
        }

        [Fact]
        public void CalculateOfficialRatingChartWithoutMovies()
        {
            var service = CreateMovieService(_settingsServiceMock);
            var stat = service.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Count.Should().Be(4);
            stat.Charts.Any(x => x.Title == Constants.CountPerOfficialRating).Should().BeTrue();

            var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerOfficialRating);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            graph.DataSets.Should().Be("[]");
        }

        [Fact]
        public void CalculateRatingChart()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Count.Should().Be(4);
            stat.Charts.Any(x => x.Title == Constants.CountPerCommunityRating).Should().BeTrue();

            var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerCommunityRating);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            var dataSet = "{\"Label\":\"0\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 0.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"1\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 1.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":1},";
            dataSet += "{\"Label\":\"2\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 2.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"3\",\"Val0\":2},";
            dataSet += "{\"Label\":\"" + 3.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"4\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 4.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"5\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 5.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"6\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 6.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"7\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 7.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"8\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 8.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"9\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 9.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0}";
            graph.DataSets.Should().Be("[" +dataSet + "]");
        }

        [Fact]
        public void CalculateRatingChartWithoutMovies()
        {
            var service = CreateMovieService(_settingsServiceMock);
            var stat = service.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.Count.Should().Be(4);
            stat.Charts.Any(x => x.Title == Constants.CountPerCommunityRating).Should().BeTrue();

            var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerCommunityRating);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            var dataSet = "{\"Label\":\"0\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 0.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"1\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 1.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"2\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 2.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"3\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 3.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"4\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 4.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"5\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 5.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"6\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 6.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"7\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 7.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"8\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 8.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0},";
            dataSet += "{\"Label\":\"9\",\"Val0\":0},";
            dataSet += "{\"Label\":\"" + 9.5.ToString(CultureInfo.CurrentCulture) + "\",\"Val0\":0}";
            graph.DataSets.Should().Be("[" + dataSet + "]");
        }

        [Fact]
        public void CalculatePremiereYearChart()
        {
            var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1991, 3, 12)).Build();
            var movieFive = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1992, 3, 12)).Build();
            var movieSix = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1989, 3, 12)).Build();
            var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour, movieFive, movieSix);

            var stat = service.GetStatistics(_collections.Select(x => x.Id).ToList());
            stat.Should().NotBeNull();
            stat.Charts.Count.Should().Be(4);
            stat.Charts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

            var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerPremiereYear);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            var dataSet = "{\"Label\":\"1985 - 1989\",\"Val0\":1},";
            dataSet += "{\"Label\":\"1990 - 1994\",\"Val0\":2},";
            dataSet += "{\"Label\":\"1995 - 1999\",\"Val0\":0},";
            dataSet += "{\"Label\":\"2000 - 2004\",\"Val0\":3}";
            graph.DataSets.Should().Be("[" + dataSet + "]");
        }

        [Fact]
        public void CalculatePremiereYearChartWithoutMovies()
        {
            var service = CreateMovieService(_settingsServiceMock);

            var stat = service.GetStatistics(_collections.Select(x => x.Id).ToList());
            stat.Should().NotBeNull();
            stat.Charts.Count.Should().Be(4);
            stat.Charts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

            var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerPremiereYear);
            graph.Should().NotBeNull();
            graph.SeriesCount.Should().Be(1);
            graph.DataSets.Should().Be("[]");
        }

        #endregion

        #region People

        [Fact]
        public void TotalTypeCountForActors()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());
            stat.People.Cards.Count(x => x.Title == Constants.Common.TotalActors).Should().Be(1);

            var card = stat.People.Cards.First(x => x.Title == Constants.Common.TotalActors);
            card.Should().NotBeNull();
            card.Value.Should().Be("4");
            card.Title.Should().Be(Constants.Common.TotalActors);
        }

        [Fact]
        public void TotalTypeCountForDirectors()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.Cards.Count(x => x.Title == Constants.Common.TotalDirectors).Should().Be(1);

            var card = stat.People.Cards.First(x => x.Title == Constants.Common.TotalDirectors);
            card.Should().NotBeNull();
            card.Value.Should().Be("2");
            card.Title.Should().Be(Constants.Common.TotalDirectors);
        }

        [Fact]
        public void TotalTypeCountForWriters()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());
            stat.People.Cards.Count(x => x.Title == Constants.Common.TotalWriters).Should().Be(1);

            var card = stat.People.Cards.First(x => x.Title == Constants.Common.TotalWriters);
            card.Should().NotBeNull();
            card.Value.Should().Be("1");
            card.Title.Should().Be(Constants.Common.TotalWriters);
        }

        [Fact]
        public void MostFeaturedActorsPerGenre()
        {
            var stat = _subject.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.MostFeaturedActorsPerGenreCards.Should().NotBeNull();
            stat.People.MostFeaturedActorsPerGenreCards.Count.Should().Be(3);
            stat.People.MostFeaturedActorsPerGenreCards[0].Title.Should().Be("Action");
            stat.People.MostFeaturedActorsPerGenreCards[0].UnitNeedsTranslation.Should().Be(false);
            stat.People.MostFeaturedActorsPerGenreCards[0].Unit.Should().Be(string.Empty);
            stat.People.MostFeaturedActorsPerGenreCards[0].ValueType.Should().Be(ValueTypeEnum.None);
            stat.People.MostFeaturedActorsPerGenreCards[1].Title.Should().Be("Comedy");
            stat.People.MostFeaturedActorsPerGenreCards[1].UnitNeedsTranslation.Should().Be(false);
            stat.People.MostFeaturedActorsPerGenreCards[1].Unit.Should().Be(string.Empty);
            stat.People.MostFeaturedActorsPerGenreCards[1].ValueType.Should().Be(ValueTypeEnum.None);
            stat.People.MostFeaturedActorsPerGenreCards[2].Title.Should().Be("Drama");
            stat.People.MostFeaturedActorsPerGenreCards[2].UnitNeedsTranslation.Should().Be(false);
            stat.People.MostFeaturedActorsPerGenreCards[2].Unit.Should().Be(string.Empty);
            stat.People.MostFeaturedActorsPerGenreCards[2].ValueType.Should().Be(ValueTypeEnum.None);
        }

        #endregion

        #region Suspicious

        [Fact]
        public void ShortMovies()
        {
            var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(0, 1, 0).Build();
            var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = service.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Shorts.Should().NotBeNull();
            stat.Shorts.Count().Should().Be(1);
            var shortMovie = stat.Shorts.Single();

            shortMovie.Title = movieFour.Name;
            shortMovie.MediaId = movieFour.Id;
            shortMovie.Number = 0;
            shortMovie.Duration = 60;
        }

        [Fact]
        public void ShortMoviesWithSettingDisabled()
        {
            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock
                .Setup(x => x.GetUserSettings())
                .Returns(new UserSettings { ToShortMovie = 10, MovieLibraries = new List<LibraryContainer> { new () { Id = _collections[0].Id}, new() { Id = _collections[1].Id } }, ToShortMovieEnabled = false });
            var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(0, 1, 0).Build();
            var service = CreateMovieService(settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = service.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Shorts.Should().NotBeNull();
            stat.Shorts.Count().Should().Be(0);
        }

        [Fact]
        public void MoviesWithoutImdb()
        {
            var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddImdb(string.Empty).Build();
            var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = service.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.NoImdb.Should().NotBeNull();
            stat.NoImdb.Count().Should().Be(1);
            var noImdbIdMovie = stat.NoImdb.Single();

            noImdbIdMovie.Title = movieFour.Name;
            noImdbIdMovie.MediaId = movieFour.Id;
            noImdbIdMovie.Number = 0;
        }

        [Fact]
        public void MoviesWithoutPrimaryImage()
        {
            var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddPrimaryImage(string.Empty).Build();
            var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = service.GetStatistics(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.NoPrimary.Should().NotBeNull();
            stat.NoPrimary.Count().Should().Be(1);
            var noPrimaryImageMovie = stat.NoPrimary.Single();

            noPrimaryImageMovie.Title = movieFour.Name;
            noPrimaryImageMovie.MediaId = movieFour.Id;
            noPrimaryImageMovie.Number = 0;
        }

        #endregion
    }
}
