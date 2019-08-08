using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using MediaBrowser.Model.Entities;
using Moq;
using Tests.Unit.Builders;
using Xunit;
using CollectionType = EmbyStat.Common.Models.Entities.CollectionType;
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
        private readonly Movie _movieThree;
        private readonly Mock<ISettingsService> _settingsServiceMock;

        public MovieServiceTests()
        {
            _collections = new List<Collection>
            {
                new Collection{ Id = string.Empty, Name = "collection1", PrimaryImage = "image1", Type = CollectionType.Movies},
                new Collection{ Id = string.Empty, Name = "collection2", PrimaryImage = "image2", Type = CollectionType.Movies}
            };

            var actorIdOne = Guid.NewGuid();

            _movieOne = new MovieBuilder(0)
                .AddCommunityRating((float) 1.7)
                .AddOfficialRating("R")
                .AddPremiereDate(new DateTime(2002, 4, 2, 0, 0, 0))
                .AddRunTimeTicks(2, 10, 0)
                .AddName("The lord of the rings")
                .AddGenres("Action", "Drama")
                .Build();

            _movieTwo = new MovieBuilder(1)
                .AddCommunityRating((float)2.8)
                .AddOfficialRating("R")
                .AddPremiereDate(new DateTime(2003, 4, 2, 0, 0, 0))
                .AddRunTimeTicks(3, 30, 0)
                .AddName("The lord of the rings, two towers")
                .AddPerson(new ExtraPerson { Type = PersonType.Director, Name = "Frodo", Id = Guid.NewGuid().ToString() })
                .AddPerson(new ExtraPerson { Type = PersonType.Actor, Name = "Frodo", Id = actorIdOne.ToString() })
                .AddGenres("Action", "Comedy")
                .AddImdb("0002")
                .Build();

            _movieThree = new MovieBuilder(2)
                .AddCommunityRating((float)3.2)
                .AddOfficialRating("B")
                .AddPremiereDate(new DateTime(2004, 4, 2, 0, 0, 0))
                .AddRunTimeTicks(3, 50, 0)
                .AddName("The lord of the rings, return of the king")
                .AddGenres("Comedy")
                .AddPerson(new ExtraPerson{Type = PersonType.Actor,Name = "Frodo", Id = actorIdOne.ToString()})
                .AddPerson(new ExtraPerson{Type = PersonType.Director, Name = "Frodo", Id = Guid.NewGuid().ToString()})
                .AddPerson(new ExtraPerson{Type = PersonType.Writer, Name = "Frodo", Id = Guid.NewGuid().ToString()})
                .AddImdb("0003")
                .Build();

            _settingsServiceMock = new Mock<ISettingsService>();
            _settingsServiceMock.Setup(x => x.GetUserSettings())
                .Returns(new UserSettings { ToShortMovie = 10, MovieCollectionTypes = new List<CollectionType> { CollectionType.Movies }, ToShortMovieEnabled = true });
            _subject = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree);
        }

        private MovieService CreateMovieService(Mock<ISettingsService> settingsServiceMock, params Movie[] movies)
        {
            var movieRepositoryMock = new Mock<IMovieRepository>();
            movieRepositoryMock.Setup(x => x.GetAll(It.IsAny<IEnumerable<string>>())).Returns(movies);
            var collectionRepositoryMock = new Mock<ICollectionRepository>();
            collectionRepositoryMock.Setup(x => x.GetCollectionByTypes(It.IsAny<IEnumerable<CollectionType>>())).Returns(_collections);

            var personServiceMock = new Mock<IPersonService>();
            foreach (var person in movies.SelectMany(x => x.People))
            {
                personServiceMock.Setup(x => x.GetPersonByNameAsync(person.Name)).Returns(
                    Task.FromResult(new Person
                    {
                        Id = person.Id,
                        Name = person.Name,
                        BirthDate = new DateTime(2000, 1, 1),
                        Primary = "primary.jpg",
                        MovieCount = 0,
                        ShowCount = 0
                    }));
            }
            
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
            var jobRepositoryMock = new Mock<IJobRepository>();
            return new MovieService(movieRepositoryMock.Object, collectionRepositoryMock.Object, personServiceMock.Object, settingsServiceMock.Object, statisticsRepositoryMock.Object, jobRepositoryMock.Object);
        }

        #region General

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
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.MovieCount.Should().NotBeNull();
            stat.General.MovieCount.Title.Should().Be(Constants.Movies.TotalMovies);
            stat.General.MovieCount.Value.Should().Be(3);
        }

        [Fact]
        public async void GetGenreCountStat()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.GenreCount.Should().NotBeNull();
            stat.General.GenreCount.Title.Should().Be(Constants.Movies.TotalGenres);
            stat.General.GenreCount.Value.Should().Be(3);
        }

        [Fact]
        public async void GetLowestRatedStat()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.LowestRatedMovie.Should().NotBeNull();
            stat.General.LowestRatedMovie.Title.Should().Be(Constants.Movies.LowestRated);
            stat.General.LowestRatedMovie.Name.Should().Be(_movieOne.Name);
            stat.General.LowestRatedMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.LowestRatedMovie.DurationMinutes.Should().Be(130);
            stat.General.LowestRatedMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.LowestRatedMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.LowestRatedMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.LowestRatedMovie.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetHighestRatedStat()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.HighestRatedMovie.Should().NotBeNull();
            stat.General.HighestRatedMovie.Title.Should().Be(Constants.Movies.HighestRated);
            stat.General.HighestRatedMovie.Name.Should().Be(_movieThree.Name);
            stat.General.HighestRatedMovie.CommunityRating.Should().Be(_movieThree.CommunityRating.ToString());
            stat.General.HighestRatedMovie.DurationMinutes.Should().Be(230);
            stat.General.HighestRatedMovie.MediaId.Should().Be(_movieThree.Id);
            stat.General.HighestRatedMovie.OfficialRating.Should().Be(_movieThree.OfficialRating);
            stat.General.HighestRatedMovie.Tag.Should().Be(_movieThree.Primary);
            stat.General.HighestRatedMovie.Year.Should().Be(_movieThree.PremiereDate.Value.Year);
        }

        [Fact]
        public async void GetOldestPremieredStat()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.OldestPremieredMovie.Should().NotBeNull();
            stat.General.OldestPremieredMovie.Title.Should().Be(Constants.Movies.OldestPremiered);
            stat.General.OldestPremieredMovie.Name.Should().Be(_movieOne.Name);
            stat.General.OldestPremieredMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.OldestPremieredMovie.DurationMinutes.Should().Be(130);
            stat.General.OldestPremieredMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.OldestPremieredMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.OldestPremieredMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.OldestPremieredMovie.Year.Should().Be(2002);
        }

        [Fact]
        public async void GetNewestPremieredStat()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.NewestPremieredMovie.Should().NotBeNull();
            stat.General.NewestPremieredMovie.Title.Should().Be(Constants.Movies.NewestPremiered);
            stat.General.NewestPremieredMovie.Name.Should().Be(_movieThree.Name);
            stat.General.NewestPremieredMovie.CommunityRating.Should().Be(_movieThree.CommunityRating.ToString());
            stat.General.NewestPremieredMovie.DurationMinutes.Should().Be(230);
            stat.General.NewestPremieredMovie.MediaId.Should().Be(_movieThree.Id);
            stat.General.NewestPremieredMovie.OfficialRating.Should().Be(_movieThree.OfficialRating);
            stat.General.NewestPremieredMovie.Tag.Should().Be(_movieThree.Primary);
            stat.General.NewestPremieredMovie.Year.Should().Be(_movieThree.PremiereDate.Value.Year);
        }

        [Fact]
        public async void GetShortestStat()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.ShortestMovie.Should().NotBeNull();
            stat.General.ShortestMovie.Title.Should().Be(Constants.Movies.Shortest);
            stat.General.ShortestMovie.Name.Should().Be(_movieOne.Name);
            stat.General.ShortestMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.ShortestMovie.DurationMinutes.Should().Be(130);
            stat.General.ShortestMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.ShortestMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.ShortestMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.ShortestMovie.Year.Should().Be(_movieOne.PremiereDate.Value.Year);
        }

        [Fact]
        public async void GetLongestStat()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.LongestMovie.Should().NotBeNull();
            stat.General.LongestMovie.Title.Should().Be(Constants.Movies.Longest);
            stat.General.LongestMovie.Name.Should().Be(_movieThree.Name);
            stat.General.LongestMovie.CommunityRating.Should().Be(_movieThree.CommunityRating.ToString());
            stat.General.LongestMovie.DurationMinutes.Should().Be(230);
            stat.General.LongestMovie.MediaId.Should().Be(_movieThree.Id);
            stat.General.LongestMovie.OfficialRating.Should().Be(_movieThree.OfficialRating);
            stat.General.LongestMovie.Tag.Should().Be(_movieThree.Primary);
            stat.General.LongestMovie.Year.Should().Be(_movieThree.PremiereDate.Value.Year);
        }

        [Fact]
        public async void GetLatestAddedStat()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.LatestAddedMovie.Should().NotBeNull();
            stat.General.LatestAddedMovie.Title.Should().Be(Constants.Movies.LatestAdded);
            stat.General.LatestAddedMovie.Name.Should().Be(_movieOne.Name);
            stat.General.LatestAddedMovie.CommunityRating.Should().Be(_movieOne.CommunityRating.ToString());
            stat.General.LatestAddedMovie.DurationMinutes.Should().Be(130);
            stat.General.LatestAddedMovie.MediaId.Should().Be(_movieOne.Id);
            stat.General.LatestAddedMovie.OfficialRating.Should().Be(_movieOne.OfficialRating);
            stat.General.LatestAddedMovie.Tag.Should().Be(_movieOne.Primary);
            stat.General.LatestAddedMovie.Year.Should().Be(2002);
        }

        #endregion

        #region Charts

        [Fact]
        public async void GetTotalPlayLengthStat()
        {
            var movieFour = new MovieBuilder(3).AddRunTimeTicks(56, 34, 1).Build();
            var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = await service.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.General.Should().NotBeNull();
            stat.General.TotalPlayableTime.Should().NotBeNull();
            stat.General.TotalPlayableTime.Title.Should().Be(Constants.Movies.TotalPlayLength);
            stat.General.TotalPlayableTime.Days.Should().Be(2);
            stat.General.TotalPlayableTime.Hours.Should().Be(18);
            stat.General.TotalPlayableTime.Minutes.Should().Be(4);
        }

        [Fact]
        public async void CalculateGenreChart()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerGenre).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerGenre);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(3);
            var labels = graph.Labels.ToArray();

            labels[0].Should().Be("Action");
            labels[1].Should().Be("Comedy");
            labels[2].Should().Be("Drama");

            graph.DataSets.Count.Should().Be(1);

            var dataset = graph.DataSets.Single().ToList();
            dataset.Count.Should().Be(3);
            dataset[0].Should().Be(2);
            dataset[1].Should().Be(2);
            dataset[2].Should().Be(1);
        }

        [Fact]
        public async void CalculateOfficialRatingChart()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerOfficialRating).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerOfficialRating);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(2);
            graph.Labels.ToList()[0].Should().Be("B");
            graph.Labels.ToList()[1].Should().Be("R");

            var dataset = graph.DataSets.Single().ToList();
            dataset.Count.Should().Be(2);
            dataset[0].Should().Be(1);
            dataset[1].Should().Be(2);
        }

        [Fact]
        public async void CalculateOfficialRatingChartWithoutMovies()
        {
            var service = CreateMovieService(_settingsServiceMock);
            var stat = await service.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerOfficialRating).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerOfficialRating);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(0);

            var dataset = graph.DataSets.Single().ToList();
            dataset.Count.Should().Be(0);
        }

        [Fact]
        public async void CalculateRatingChart()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerCommunityRating).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerCommunityRating);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(20);
            for (var i = 0; i < 20; i++)
            {
                graph.Labels.ToArray()[i].Should().Be((i * (float)0.5).ToString());
            }

            var dataSet = graph.DataSets.Single().ToList();
            dataSet.Count.Should().Be(20);
            dataSet[0].Should().Be(0);
            dataSet[1].Should().Be(0);
            dataSet[2].Should().Be(0);
            dataSet[3].Should().Be(1);
            dataSet[4].Should().Be(0);
            dataSet[5].Should().Be(0);
            dataSet[6].Should().Be(2);
            dataSet[7].Should().Be(0);
            for (var i = 8; i < 20; i++)
            {
                dataSet[i].Should().Be(0);
            }
        }

        [Fact]
        public async void CalculateRatingChartWithoutMovies()
        {
            var service = CreateMovieService(_settingsServiceMock);
            var stat = await service.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerCommunityRating).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerCommunityRating);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(20);
            for (var i = 0; i < 20; i++)
            {
                graph.Labels.ToArray()[i].Should().Be((i * (float)0.5).ToString());
            }

            var dataset = graph.DataSets.Single().ToList();
            dataset.Count.Should().Be(20);
            for (var i = 0; i < 20; i++)
            {
                dataset[i].Should().Be(0);
            }
        }

        [Fact]
        public async void CalculatePremiereYearChart()
        {
            var movieFour = new MovieBuilder(3).AddPremiereDate(new DateTime(1991, 3, 12)).Build();
            var movieFive = new MovieBuilder(4).AddPremiereDate(new DateTime(1992, 3, 12)).Build();
            var movieSix = new MovieBuilder(5).AddPremiereDate(new DateTime(1989, 3, 12)).Build();
            var service = CreateMovieService(_settingsServiceMock,_movieOne, _movieTwo, _movieThree, movieFour, movieFive, movieSix);

            var stat = await service.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());
            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerPremiereYear);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(4);
            graph.Labels.ToArray()[0].Should().Be("1985 - 1989");
            graph.Labels.ToArray()[1].Should().Be("1990 - 1994");
            graph.Labels.ToArray()[2].Should().Be("1995 - 1999");
            graph.Labels.ToArray()[3].Should().Be("2000 - 2004");

            var dataset = graph.DataSets.Single().ToList();
            dataset.Count.Should().Be(4);
            dataset[0].Should().Be(1);
            dataset[1].Should().Be(2);
            dataset[2].Should().Be(0);
            dataset[3].Should().Be(3);
        }

        [Fact]
        public async void CalculatePremiereYearChartWithoutMovies()
        {
            var service = CreateMovieService(_settingsServiceMock);

            var stat = await service.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());
            stat.Should().NotBeNull();
            stat.Charts.BarCharts.Count.Should().Be(4);
            stat.Charts.BarCharts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

            var graph = stat.Charts.BarCharts.SingleOrDefault(x => x.Title == Constants.CountPerPremiereYear);
            graph.Should().NotBeNull();
            graph.Labels.Count().Should().Be(0);

            var dataset = graph.DataSets.Single().ToList();
            dataset.Count.Should().Be(0);
        }

        #endregion

        #region People

        [Fact]
        public async void TotalTypeCountForActors()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.TotalActorCount.Should().NotBeNull();
            stat.People.TotalActorCount.Value.Should().Be(4);
            stat.People.TotalActorCount.Title.Should().Be(Constants.Common.TotalActors);
        }

        [Fact]
        public async void TotalTypeCountForDirectors()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.TotalDirectorCount.Should().NotBeNull();
            stat.People.TotalDirectorCount.Value.Should().Be(2);
            stat.People.TotalDirectorCount.Title.Should().Be(Constants.Common.TotalDirectors);
        }

        [Fact]
        public async void TotalTypeCountForWriters()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.TotalWriterCount.Should().NotBeNull();
            stat.People.TotalWriterCount.Value.Should().Be(1);
            stat.People.TotalWriterCount.Title.Should().Be(Constants.Common.TotalWriters);
        }

        [Fact]
        public async void MostFeaturedActorsPerGenre()
        {
            var stat = await _subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.People.Should().NotBeNull();
            stat.People.MostFeaturedActorsPerGenre.Should().NotBeNull();
            stat.People.MostFeaturedActorsPerGenre.Count.Should().Be(3);
            stat.People.MostFeaturedActorsPerGenre[0].Title.Should().Be("Action");
            stat.People.MostFeaturedActorsPerGenre[0].Name.Should().Be("Gimli");
            stat.People.MostFeaturedActorsPerGenre[1].Title.Should().Be("Comedy");
            stat.People.MostFeaturedActorsPerGenre[1].Name.Should().Be("Gimli");
            stat.People.MostFeaturedActorsPerGenre[2].Title.Should().Be("Drama");
            stat.People.MostFeaturedActorsPerGenre[2].Name.Should().Be("Gimli");
        }

        #endregion

        #region Suspicious

        [Fact]
        public async void DuplicateMovies()
        {
            var movieFour = new MovieBuilder(3).AddImdb(_movieOne.IMDB).Build();
            var subject = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = await subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Suspicious.Should().NotBeNull();
            stat.Suspicious.Duplicates.Should().NotBeNull();
            stat.Suspicious.Duplicates.Count().Should().Be(1);
            var duplicate = stat.Suspicious.Duplicates.Single();

            duplicate.ItemOne.Id.Should().Be(0);
            duplicate.ItemTwo.Id.Should().Be(3);

            duplicate.Number.Should().Be(0);
            duplicate.Title.Should().Be(_movieOne.Name);
            duplicate.Reason.Should().Be(Constants.ByImdb);
        }

        [Fact]
        public async void DuplicateMoviesButDfferent3DFormat()
        {
            var movieFour = new MovieBuilder(3).AddVideo3DFormat(Video3DFormat.FullSideBySide).Build();
            var subject = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = await subject.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Suspicious.Should().NotBeNull();
            stat.Suspicious.Duplicates.Should().NotBeNull();
            stat.Suspicious.Duplicates.Count().Should().Be(0);
        }

        [Fact]
        public async void ShortMovies()
        {
            var movieFour = new MovieBuilder(3).AddRunTimeTicks(0, 1, 0).Build();
            var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = await service.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Suspicious.Should().NotBeNull();
            stat.Suspicious.Shorts.Should().NotBeNull();
            stat.Suspicious.Shorts.Count().Should().Be(1);
            var shortMovie = stat.Suspicious.Shorts.Single();

            shortMovie.Title = movieFour.Name;
            shortMovie.MediaId = movieFour.Id;
            shortMovie.Number = 0;
            shortMovie.Duration = 60;
        }

        [Fact]
        public async void ShortMoviesWithSettingDisabled()
        {
            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock.Setup(x => x.GetUserSettings())
                .Returns(new UserSettings { ToShortMovie = 10, MovieCollectionTypes = new List<CollectionType> { CollectionType.Movies }, ToShortMovieEnabled = false });
            var movieFour = new MovieBuilder(3).AddRunTimeTicks(0, 1, 0).Build();
            var service = CreateMovieService(settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = await service.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Suspicious.Should().NotBeNull();
            stat.Suspicious.Shorts.Should().NotBeNull();
            stat.Suspicious.Shorts.Count().Should().Be(0);
        }

        [Fact]
        public async void MoviesWithoutImdb()
        {
            var movieFour = new MovieBuilder(4).AddImdb(string.Empty).Build();
            var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = await service.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Suspicious.Should().NotBeNull();
            stat.Suspicious.NoImdb.Should().NotBeNull();
            stat.Suspicious.NoImdb.Count().Should().Be(1);
            var noImdbIdMovie = stat.Suspicious.NoImdb.Single();

            noImdbIdMovie.Title = movieFour.Name;
            noImdbIdMovie.MediaId = movieFour.Id;
            noImdbIdMovie.Number = 0;
        }

        [Fact]
        public async void MoviesWithoutPrimaryImage()
        {
            var movieFour = new MovieBuilder(4).AddPrimaryImage(string.Empty).Build();
            var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
            var stat = await service.GetMovieStatisticsAsync(_collections.Select(x => x.Id).ToList());

            stat.Suspicious.Should().NotBeNull();
            stat.Suspicious.NoPrimary.Should().NotBeNull();
            stat.Suspicious.NoPrimary.Count().Should().Be(1);
            var noPrimaryImageMovie = stat.Suspicious.NoPrimary.Single();

            noPrimaryImageMovie.Title = movieFour.Name;
            noPrimaryImageMovie.MediaId = movieFour.Id;
            noPrimaryImageMovie.Number = 0;
        }

        #endregion
    }
}
