using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Query;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Movies;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.Statistics;
using EmbyStat.Core.Statistics.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Tests.Unit.Builders;
using Xunit;
using ValueType = EmbyStat.Common.Models.Cards.ValueType;

namespace Tests.Unit.Services;

public class MovieStatisticServiceTests
{
    private readonly MovieStatisticService _subject;
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly Mock<ILogger<MovieStatisticService>> _logger;
    private readonly Mock<IConfigurationService> _configurationServiceMock;

    private readonly Movie _movieOne;
    private readonly Movie _movieTwo;
    private readonly Movie _movieThree;

    public MovieStatisticServiceTests()
    {
        var actorIdOne = 1;

        _movieRepositoryMock = new Mock<IMovieRepository>();
        _logger = new Mock<ILogger<MovieStatisticService>>();
        _configurationServiceMock = new Mock<IConfigurationService>();

        _movieOne = new MovieBuilder(Guid.NewGuid().ToString())
            .AddCommunityRating(1.7M)
            .AddOfficialRating("R")
            .AddPremiereDate(new DateTime(2002, 4, 2, 0, 0, 0))
            .AddRunTimeTicks(2, 10, 0)
            .AddName("The lord of the rings")
            .AddSortName("The-lord-of-the-rings")
            .AddGenres(new Genre {Name = "Action"}, new Genre {Name = "Drama"})
            .Build();

        _movieTwo = new MovieBuilder(Guid.NewGuid().ToString())
            .AddCommunityRating(2.8M)
            .AddOfficialRating("R")
            .AddPremiereDate(new DateTime(2003, 4, 2, 0, 0, 0))
            .AddRunTimeTicks(3, 30, 0)
            .AddName("The lord of the rings, two towers")
            .AddSortName("The-lord-of-the-rings,-two-towers")
            .AddPerson(new MediaPerson {Type = PersonType.Director, Person = new Person {Name = "Gimli"}, Id = 2})
            .AddPerson(new MediaPerson {Type = PersonType.Actor, Person = new Person {Name = "Frodo"}, Id = actorIdOne})
            .AddGenres(new Genre {Name = "Action"}, new Genre {Name = "Comedy"})
            .AddImdb("0002")
            .Build();

        _movieThree = new MovieBuilder(Guid.NewGuid().ToString())
            .AddCommunityRating(3.2M)
            .AddOfficialRating("B")
            .AddPremiereDate(new DateTime(2004, 4, 2, 0, 0, 0))
            .AddRunTimeTicks(3, 50, 0)
            .AddName("The lord of the rings, return of the king")
            .AddSortName("The-lord-of-the-rings,-return-of-the-king")
            .AddGenres(new Genre {Name = "Comedy"})
            .AddPerson(new MediaPerson {Type = PersonType.Actor, Person = new Person {Name = "Frodo"}, Id = actorIdOne})
            .AddPerson(new MediaPerson {Type = PersonType.Director, Person = new Person {Name = "Frodo"}, Id = 4})
            .AddPerson(new MediaPerson {Type = PersonType.Writer, Person = new Person {Name = "Frodo"}, Id = 5})
            .AddImdb("0003")
            .Build();
    }

    private MovieStatisticService CreateService(Action registerMocks, params Movie[] movies)
    {
        registerMocks();
        return new MovieStatisticService(_movieRepositoryMock.Object, _logger.Object, _configurationServiceMock.Object);
    }

    [Fact]
    public async Task CalculateTotalMovieCount_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.Count())
                .ReturnsAsync(movies.Length);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.MovieTotalCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.TheatersRoundedIcon);
        obj.Title.Should().Be(Constants.Movies.TotalMovies);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("3");

        _movieRepositoryMock.Verify(x => x.Count(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalMovieGenres_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetGenreCount())
                .ReturnsAsync(movies.SelectMany(x => x.Genres).Select(x => x.Name).Distinct().Count());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.MovieTotalGenreCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.PoundRoundedIcon);
        obj.Title.Should().Be(Constants.Common.TotalGenres);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("3");

        _movieRepositoryMock.Verify(x => x.GetGenreCount(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalPlayLength_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetTotalRuntime())
                .ReturnsAsync(movies.Sum(x => x.RunTimeTicks ?? 0));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.MovieTotalPlayLength);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.QueryBuilderRoundedIcon);
        obj.Title.Should().Be(Constants.Movies.TotalPlayLength);
        obj.Type.Should().Be(CardType.Time);
        obj.Value.Should().Be("0|9|30");

        _movieRepositoryMock.Verify(x => x.GetTotalRuntime(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalPlayLength_Should_Default_To_Zero()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetTotalRuntime())
                .ReturnsAsync((long?) null);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.MovieTotalPlayLength);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.QueryBuilderRoundedIcon);
        obj.Title.Should().Be(Constants.Movies.TotalPlayLength);
        obj.Type.Should().Be(CardType.Time);
        obj.Value.Should().Be("0|0|0");

        _movieRepositoryMock.Verify(x => x.GetTotalRuntime(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalDiskSpace_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetTotalDiskSpace())
                .ReturnsAsync(movies.Sum(x => x.MediaSources.FirstOrDefault()?.SizeInMb ?? 0));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.MovieTotalDiskSpaceUsage);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.StorageRoundedIcon);
        obj.Title.Should().Be(Constants.Common.TotalDiskSpace);
        obj.Type.Should().Be(CardType.Size);
        obj.Value.Should().Be("6000");

        _movieRepositoryMock.Verify(x => x.GetTotalDiskSpace(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalWatchedMovies_Should_Calculate()
    {
        void Registrations()
        {
            _movieRepositoryMock
                .Setup(x => x.GetTotalWatchedMovieCount())
                .ReturnsAsync(2);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.MovieTotalWatchedCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.PoundRoundedIcon);
        obj.Title.Should().Be(Constants.Movies.TotalWatchedMovies);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("2");

        _movieRepositoryMock.Verify(x => x.GetTotalWatchedMovieCount(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalWatchedTime_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetPlayedRuntime())
                .ReturnsAsync(movies.Sum(x => x.RunTimeTicks ?? 0));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.MovieTotalWatchedTime);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.QueryBuilderRoundedIcon);
        obj.Title.Should().Be(Constants.Movies.PlayedPlayLength);
        obj.Type.Should().Be(CardType.Time);
        obj.Value.Should().Be("0|9|30");

        _movieRepositoryMock.Verify(x => x.GetPlayedRuntime(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TotalPersonTypeCount_Should_Calculate_Actors()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetPeopleCount(PersonType.Actor))
                .ReturnsAsync(movies.SelectMany(x => x.People).DistinctBy(x => x.Id)
                    .Count(x => x.Type == PersonType.Actor));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.MovieTotalActorCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.PeopleAltRoundedIcon);
        obj.Title.Should().Be(Constants.Common.TotalActors);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("1");

        _movieRepositoryMock.Verify(x => x.GetPeopleCount(PersonType.Actor), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateTotalCurrentWatchingCount_Should_Calculate()
    {
        void Registrations()
        {
            _movieRepositoryMock
                .Setup(x => x.GetCurrentWatchingCount())
                .ReturnsAsync(2);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.Card, Statistic.MovieTotalCurrentWatchingCount);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<Card>(result);
        obj.Should().NotBeNull();
        obj.Icon.Should().Be(Constants.Icons.PlayRoundedIcon);
        obj.Title.Should().Be(Constants.Movies.CurrentPlayingCount);
        obj.Type.Should().Be(CardType.Text);
        obj.Value.Should().Be("2");

        _movieRepositoryMock.Verify(x => x.GetCurrentWatchingCount(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HighestRatedMovies_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetHighestRatedMedia(5))
                .ReturnsAsync(movies.OrderByDescending(x => x.CommunityRating));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.MovieHighestRatedList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Movies.HighestRated);
        obj.Unit.Should().Be("/10");
        obj.UnitNeedsTranslation.Should().BeFalse();
        obj.Values.Length.Should().Be(3);
        obj.Values[0].Value.Should().Be(_movieThree.CommunityRating.ToString());
        obj.Values[0].Label.Should().Be(_movieThree.Name);
        obj.ValueType.Should().Be(ValueType.None);

        _movieRepositoryMock.Verify(x => x.GetHighestRatedMedia(5), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LowestRatedMovies_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetLowestRatedMedia(5))
                .ReturnsAsync(movies.OrderBy(x => x.CommunityRating));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.MovieLowestRatedList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Movies.LowestRated);
        obj.Unit.Should().Be("/10");
        obj.UnitNeedsTranslation.Should().BeFalse();
        obj.Values.Length.Should().Be(3);
        obj.Values[0].Value.Should().Be(_movieOne.CommunityRating.ToString());
        obj.Values[0].Label.Should().Be(_movieOne.Name);
        obj.ValueType.Should().Be(ValueType.None);

        _movieRepositoryMock.Verify(x => x.GetLowestRatedMedia(5), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task OldestPremieredMovies_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetOldestPremieredMedia(5))
                .ReturnsAsync(movies.OrderBy(x => x.PremiereDate));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.MovieOldestPremieredList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Movies.OldestPremiered);
        obj.Unit.Should().Be("COMMON.DATE");
        obj.UnitNeedsTranslation.Should().BeTrue();
        obj.Values.Length.Should().Be(3);
        obj.Values[0].Value.Should().Be(_movieOne.PremiereDate?.ToString("s"));
        obj.Values[0].Label.Should().Be(_movieOne.Name);
        obj.ValueType.Should().Be(ValueType.Date);

        _movieRepositoryMock.Verify(x => x.GetOldestPremieredMedia(5), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task NewestPremieredMovies_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetNewestPremieredMedia(5))
                .ReturnsAsync(movies.OrderByDescending(x => x.PremiereDate));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.MovieNewestPremieredList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Movies.NewestPremiered);
        obj.Unit.Should().Be("COMMON.DATE");
        obj.UnitNeedsTranslation.Should().BeTrue();
        obj.Values.Length.Should().Be(3);
        obj.Values[0].Value.Should().Be(_movieThree.PremiereDate?.ToString("s"));
        obj.Values[0].Label.Should().Be(_movieThree.Name);
        obj.ValueType.Should().Be(ValueType.Date);

        _movieRepositoryMock.Verify(x => x.GetNewestPremieredMedia(5), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShortestMovies_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetShortestMovie(6000000000, 5))
                .ReturnsAsync(movies.OrderBy(x => x.RunTimeTicks).ToList());

            var config = new ConfigBuilder()
                .EnableToShortMovies(true, 10)
                .Build();
            _configurationServiceMock.Setup(x => x.Get())
                .Returns(config);
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.MovieShortestList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Movies.Shortest);
        obj.Unit.Should().Be("COMMON.MIN");
        obj.UnitNeedsTranslation.Should().BeTrue();
        obj.Values.Length.Should().Be(3);
        obj.Values[0].Value.Should().Be(_movieOne.RunTimeTicks.ToString());
        obj.Values[0].Label.Should().Be(_movieOne.Name);
        obj.ValueType.Should().Be(ValueType.Ticks);

        _movieRepositoryMock.Verify(x => x.GetShortestMovie(6000000000, 5), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.Verify(x => x.Get(), Times.Once);
        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LongestMovies_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetLongestMovie(5))
                .ReturnsAsync(movies.OrderByDescending(x => x.RunTimeTicks).ToList());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.MovieLongestList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Movies.Longest);
        obj.Unit.Should().Be("COMMON.MIN");
        obj.UnitNeedsTranslation.Should().BeTrue();
        obj.Values.Length.Should().Be(3);
        obj.Values[0].Value.Should().Be(_movieThree.RunTimeTicks.ToString());
        obj.Values[0].Label.Should().Be(_movieThree.Name);
        obj.ValueType.Should().Be(ValueType.Ticks);

        _movieRepositoryMock.Verify(x => x.GetLongestMovie(5), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LatestAddedMovies_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetLatestAddedMovie(5))
                .ReturnsAsync(movies.OrderByDescending(x => x.DateCreated).ToList());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.MovieLatestAddedList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Movies.LatestAdded);
        obj.Unit.Should().Be("COMMON.DATE");
        obj.UnitNeedsTranslation.Should().BeTrue();
        obj.Values.Length.Should().Be(3);
        obj.Values[0].Value.Should().Be(_movieOne.DateCreated?.ToString("s"));
        obj.Values[0].Label.Should().Be(_movieOne.Name);
        obj.ValueType.Should().Be(ValueType.Date);

        _movieRepositoryMock.Verify(x => x.GetLatestAddedMovie(5), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task MostWatchedMovies_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetMostWatchedMovies(5))
                .ReturnsAsync(movies.ToDictionary(x => x, x => x.Genres.Count));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.TopCard, Statistic.MovieMostWatchedList);

        result.Should().NotBeNullOrWhiteSpace();
        var obj = JsonConvert.DeserializeObject<TopCard>(result);
        obj.Should().NotBeNull();
        obj.Title.Should().Be(Constants.Movies.MostWatchedMovies);
        obj.Unit.Should().Be("#");
        obj.UnitNeedsTranslation.Should().BeFalse();
        obj.Values.Length.Should().Be(3);
        obj.Values[0].Value.Should().Be(_movieOne.Genres.Count.ToString());
        obj.Values[0].Label.Should().Be(_movieOne.Name);
        obj.ValueType.Should().Be(ValueType.None);

        _movieRepositoryMock.Verify(x => x.GetMostWatchedMovies(5), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateGenreChart_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetGenreChartValues())
                .ReturnsAsync(
                    movies
                        .SelectMany(x => x.Genres)
                        .GroupBy(x => x.Name)
                        .OrderBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Count())
                );
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.BarChart, Statistic.MovieGenreChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.CountPerGenre);
        graph!.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(3);
        graph.DataSets[0].Label.Should().Be("Action");
        graph.DataSets[0].Value.Should().Be(2);
        graph.DataSets[1].Label.Should().Be("Comedy");
        graph.DataSets[1].Value.Should().Be(2);
        graph.DataSets[2].Label.Should().Be("Drama");
        graph.DataSets[2].Value.Should().Be(1);

        _movieRepositoryMock.Verify(x => x.GetGenreChartValues(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculateRatingChart_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetCommunityRatings())
                .ReturnsAsync(movies.Select(x => x.CommunityRating).ToList());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.BarChart, Statistic.MovieRatingChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.CountPerCommunityRating);
        graph!.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(20);
        graph.DataSets[0].Label.Should().Be("0");
        graph.DataSets[0].Value.Should().Be(0);
        graph.DataSets[1].Label.Should().Be(0.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[1].Value.Should().Be(0);
        graph.DataSets[2].Label.Should().Be("1");
        graph.DataSets[2].Value.Should().Be(0);
        graph.DataSets[3].Label.Should().Be(1.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[3].Value.Should().Be(1);
        graph.DataSets[4].Label.Should().Be("2");
        graph.DataSets[4].Value.Should().Be(0);
        graph.DataSets[5].Label.Should().Be(2.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[5].Value.Should().Be(0);
        graph.DataSets[6].Label.Should().Be("3");
        graph.DataSets[6].Value.Should().Be(2);
        graph.DataSets[7].Label.Should().Be(3.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[7].Value.Should().Be(0);
        graph.DataSets[8].Label.Should().Be("4");
        graph.DataSets[8].Value.Should().Be(0);
        graph.DataSets[9].Label.Should().Be(4.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[9].Value.Should().Be(0);
        graph.DataSets[10].Label.Should().Be("5");
        graph.DataSets[10].Value.Should().Be(0);
        graph.DataSets[11].Label.Should().Be(5.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[11].Value.Should().Be(0);
        graph.DataSets[12].Label.Should().Be("6");
        graph.DataSets[12].Value.Should().Be(0);
        graph.DataSets[13].Label.Should().Be(6.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[13].Value.Should().Be(0);
        graph.DataSets[14].Label.Should().Be("7");
        graph.DataSets[14].Value.Should().Be(0);
        graph.DataSets[15].Label.Should().Be(7.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[15].Value.Should().Be(0);
        graph.DataSets[16].Label.Should().Be("8");
        graph.DataSets[16].Value.Should().Be(0);
        graph.DataSets[17].Label.Should().Be(8.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[17].Value.Should().Be(0);
        graph.DataSets[18].Label.Should().Be("9");
        graph.DataSets[18].Value.Should().Be(0);
        graph.DataSets[19].Label.Should().Be(9.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[19].Value.Should().Be(0);

        _movieRepositoryMock.Verify(x => x.GetCommunityRatings(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculatePremiereYearChart_Should_Calculate()
    {
        void Registrations()
        {
            var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1991, 3, 12))
                .Build();
            var movieFive = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1992, 3, 12))
                .Build();
            var movieSix = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1989, 3, 12))
                .Build();
            
            var movies = new[] {_movieOne, _movieTwo, _movieThree, movieFour, movieFive, movieSix};
            _movieRepositoryMock
                .Setup(x => x.GetPremiereYears())
                .ReturnsAsync(movies.Select(x => x.PremiereDate).ToList());
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.BarChart, Statistic.MoviePremiereYearChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.CountPerPremiereYear);
        graph!.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(4);
        graph.DataSets[0].Label.Should().Be("1985 - 1989");
        graph.DataSets[0].Value.Should().Be(1);
        graph.DataSets[1].Label.Should().Be("1990 - 1994");
        graph.DataSets[1].Value.Should().Be(2);
        graph.DataSets[2].Label.Should().Be("1995 - 1999");
        graph.DataSets[2].Value.Should().Be(0);
        graph.DataSets[3].Label.Should().Be("2000 - 2004");
        graph.DataSets[3].Value.Should().Be(3);

        _movieRepositoryMock.Verify(x => x.GetPremiereYears(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculateOfficialRatingChart_Should_Calculate()
    {
        void Registrations()
        {
            var movies = new[] {_movieOne, _movieTwo, _movieThree};
            _movieRepositoryMock
                .Setup(x => x.GetOfficialRatingChartValues())
                .ReturnsAsync(movies
                    .Select(x => x.OfficialRating)
                    .GroupBy(x => x)
                    .ToDictionary(x => x.Key, x => x.Count()));
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.BarChart, Statistic.MovieOfficialRatingChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<Chart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.CountPerOfficialRating);
        graph!.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(2);
        graph.DataSets[0].Label.Should().Be("R");
        graph.DataSets[0].Value.Should().Be(2);
        graph.DataSets[1].Label.Should().Be("B");
        graph.DataSets[1].Value.Should().Be(1);

        _movieRepositoryMock.Verify(x => x.GetOfficialRatingChartValues(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculateWatchedPerDayOfWeekChart_Should_Calculate()
    {
        void Registrations()
        {
            _movieRepositoryMock
                .Setup(x => x.GetWatchedPerDayOfWeekChartValues())
                .ReturnsAsync(new[]
                {
                    new BarValue<string, int> { Serie = "user1", X = "2", Y = 2},
                    new BarValue<string, int> { Serie = "user1", X = "3", Y = 1},
                    new BarValue<string, int> { Serie = "user2", X = "3", Y = 2},
                });
        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.ComplexChart, Statistic.MovieWatchedPerDayOfWeekChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<ComplexChart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.Movies.DaysOfTheWeek);
        graph.Series.Length.Should().Be(2);
        graph.Series[0].Should().Be("user1");
        graph.Series[1].Should().Be("user2");
        graph.DataSets.Should().Be("[{\"label\":\"1\"},{\"label\":\"2\",\"user1\":2},{\"label\":\"3\",\"user1\":1,\"user2\":2},{\"label\":\"4\"},{\"label\":\"5\"},{\"label\":\"6\"},{\"label\":\"0\"}]");
        
        _movieRepositoryMock.Verify(x => x.GetWatchedPerDayOfWeekChartValues(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculateWatchedPerHourOfDayChart_Should_Calculate()
    {
        void Registrations()
        {
            _movieRepositoryMock
                .Setup(x => x.GetWatchedPerHourOfDayChartValues())
                .ReturnsAsync(new[]
                {
                    new BarValue<string, int> { Serie = "user1", X = "10", Y = 2},
                    new BarValue<string, int> { Serie = "user1", X = "11", Y = 1},
                    new BarValue<string, int> { Serie = "user2", X = "10", Y = 2},
                });
            
            _configurationServiceMock
                .Setup(x => x.GetLocalTimeZoneInfo())
                .Returns(TimeZoneInfo.FindSystemTimeZoneById("Europe/Brussels"));

        }

        var service = CreateService(Registrations);
        var result = await service.CalculateStatistic(StatisticCardType.ComplexChart, Statistic.MovieWatchedPerHourOfDayChart);

        result.Should().NotBeNullOrWhiteSpace();
        var graph = JsonConvert.DeserializeObject<ComplexChart>(result);
        graph.Should().NotBeNull();
        graph.Title.Should().Be(Constants.Movies.WatchedPerHour);
        graph.Series.Length.Should().Be(2);
        graph.Series[0].Should().Be("user1");
        graph.Series[1].Should().Be("user2");
        graph.DataSets.Should().Be("[{\"label\":\"0001-01-01T00:00:00\"},{\"label\":\"0001-01-01T01:00:00\"},{\"label\":\"0001-01-01T02:00:00\"},{\"label\":\"0001-01-01T03:00:00\"},{\"label\":\"0001-01-01T04:00:00\"},{\"label\":\"0001-01-01T05:00:00\"},{\"label\":\"0001-01-01T06:00:00\"},{\"label\":\"0001-01-01T07:00:00\"},{\"label\":\"0001-01-01T08:00:00\"},{\"label\":\"0001-01-01T09:00:00\"},{\"label\":\"0001-01-01T10:00:00\"},{\"label\":\"0001-01-01T11:00:00\",\"user1\":2,\"user2\":2},{\"label\":\"0001-01-01T12:00:00\",\"user1\":1},{\"label\":\"0001-01-01T13:00:00\"},{\"label\":\"0001-01-01T14:00:00\"},{\"label\":\"0001-01-01T15:00:00\"},{\"label\":\"0001-01-01T16:00:00\"},{\"label\":\"0001-01-01T17:00:00\"},{\"label\":\"0001-01-01T18:00:00\"},{\"label\":\"0001-01-01T19:00:00\"},{\"label\":\"0001-01-01T20:00:00\"},{\"label\":\"0001-01-01T21:00:00\"},{\"label\":\"0001-01-01T22:00:00\"},{\"label\":\"0001-01-01T23:00:00\"}]");
        
        _movieRepositoryMock.Verify(x => x.GetWatchedPerHourOfDayChartValues(), Times.Once);
        _movieRepositoryMock.VerifyNoOtherCalls();

        _configurationServiceMock.Verify(x => x.GetLocalTimeZoneInfo(), Times.Once);
        _configurationServiceMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalGenreCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalPlayLength)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalDiskSpaceUsage)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalWatchedCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalWatchedTime)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalActorCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalDirectorCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalWriterCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieTotalCurrentWatchingCount)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieHighestRatedList)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieLowestRatedList)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieOldestPremieredList)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieNewestPremieredList)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieShortestList)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieLongestList)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieLatestAddedList)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieMostWatchedList)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieWatchedPerHourOfDayChart)]
    [InlineData(StatisticCardType.BarChart, Statistic.MovieWatchedPerDayOfWeekChart)]
    [InlineData(StatisticCardType.Card, Statistic.MovieHighestRatedList)]
    [InlineData(StatisticCardType.Card, Statistic.MovieLowestRatedList)]
    [InlineData(StatisticCardType.Card, Statistic.MovieOldestPremieredList)]
    [InlineData(StatisticCardType.Card, Statistic.MovieNewestPremieredList)]
    [InlineData(StatisticCardType.Card, Statistic.MovieShortestList)]
    [InlineData(StatisticCardType.Card, Statistic.MovieLongestList)]
    [InlineData(StatisticCardType.Card, Statistic.MovieLatestAddedList)]
    [InlineData(StatisticCardType.Card, Statistic.MovieMostWatchedList)]
    [InlineData(StatisticCardType.Card, Statistic.MovieRatingChart)]
    [InlineData(StatisticCardType.Card, Statistic.MovieGenreChart)]
    [InlineData(StatisticCardType.Card, Statistic.MoviePremiereYearChart)]
    [InlineData(StatisticCardType.Card, Statistic.MovieOfficialRatingChart)]
    [InlineData(StatisticCardType.Card, Statistic.MovieWatchedPerHourOfDayChart)]
    [InlineData(StatisticCardType.Card, Statistic.MovieWatchedPerDayOfWeekChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieWatchedPerDayOfWeekChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalGenreCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalPlayLength)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalDiskSpaceUsage)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalWatchedCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalWatchedTime)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalActorCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalDirectorCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalWriterCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieTotalCurrentWatchingCount)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieGenreChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieRatingChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.MoviePremiereYearChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieOfficialRatingChart)]
    [InlineData(StatisticCardType.TopCard, Statistic.MovieWatchedPerHourOfDayChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalGenreCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalPlayLength)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalDiskSpaceUsage)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalWatchedCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalWatchedTime)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalActorCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalDirectorCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalWriterCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieTotalCurrentWatchingCount)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieHighestRatedList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieLowestRatedList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieOldestPremieredList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieNewestPremieredList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieShortestList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieLongestList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieLatestAddedList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieMostWatchedList)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieRatingChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieGenreChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MoviePremiereYearChart)]
    [InlineData(StatisticCardType.ComplexChart, Statistic.MovieOfficialRatingChart)]
    public async Task Should_Return_Null(StatisticCardType type, Statistic uniqueId)
    {
        var service = new MovieStatisticService(_movieRepositoryMock.Object, _logger.Object, _configurationServiceMock.Object);
        var result = await service.CalculateStatistic(type, uniqueId);
        result.Should().BeNull();
        
        _movieRepositoryMock.VerifyNoOtherCalls();
        _configurationServiceMock.VerifyNoOtherCalls();
    }
}