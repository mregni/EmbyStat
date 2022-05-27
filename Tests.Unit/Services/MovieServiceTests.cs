using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Query;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Movies;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Unit.Builders;
using Xunit;
using Constants = EmbyStat.Common.Constants;
using ValueType = EmbyStat.Common.Models.Cards.ValueType;

namespace Tests.Unit.Services;

public class MovieServiceTests
{
    private readonly MovieService _subject;
    private readonly Movie _movieOne;
    private readonly Movie _movieTwo;
    private readonly Movie _movieThree;
    private readonly Mock<IRollbarService> _settingsServiceMock;
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly Mock<IMediaServerRepository> _mediaServerRepositoryMock;
    private readonly Mock<ILogger<MovieService>> _logger;

    public MovieServiceTests()
    {
        var actorIdOne = 1;

        _movieRepositoryMock = new Mock<IMovieRepository>();
        _logger = new Mock<ILogger<MovieService>>();
        _mediaServerRepositoryMock = new Mock<IMediaServerRepository>();

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
            .AddPerson(new MediaPerson {Type = PersonType.Director, Person = new Person { Name= "Gimli"}, Id = 2})
            .AddPerson(new MediaPerson {Type = PersonType.Actor, Person = new Person { Name= "Frodo"}, Id = actorIdOne})
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
            .AddPerson(new MediaPerson {Type = PersonType.Actor, Person = new Person { Name= "Frodo"}, Id = actorIdOne})
            .AddPerson(new MediaPerson {Type = PersonType.Director, Person = new Person { Name= "Frodo"}, Id = 4})
            .AddPerson(new MediaPerson {Type = PersonType.Writer, Person = new Person { Name= "Frodo"}, Id = 5})
            .AddImdb("0003")
            .Build();

        _settingsServiceMock = new Mock<IRollbarService>();
        _settingsServiceMock
            .Setup(x => x.GetUserSettings())
            .Returns(new UserSettings
            {
                ToShortMovie = 10,
                ToShortMovieEnabled = true
            });
        _subject = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree);
    }

    private MovieService CreateMovieService(Mock<IRollbarService> settingsServiceMock, params Movie[] movies)
    {
        _movieRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(movies.ToList());
        _movieRepositoryMock
            .Setup(x => x.GetAllWithImdbId())
            .Returns(movies.ToList());
        _movieRepositoryMock
            .Setup(x => x.GetToShortMovieList(It.IsAny<int>()))
            .Returns(movies.ToList());
        _movieRepositoryMock
            .Setup(x => x.GetMoviesWithoutImdbId())
            .Returns(movies.ToList());
        _movieRepositoryMock
            .Setup(x => x.GetMoviesWithoutPrimaryImage())
            .Returns(movies.ToList());
        _movieRepositoryMock
            .Setup(x => x.GetOfficialRatingChartValues())
            .ReturnsAsync(movies
                .Select(x => x.OfficialRating)
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count()));

        _movieRepositoryMock
            .Setup(x => x.GetCommunityRatings())
            .Returns(movies.Select(x => x.CommunityRating));
        _movieRepositoryMock
            .Setup(x => x.GetGenreChartValues())
            .ReturnsAsync(
                movies
                    .SelectMany(x => x.Genres)
                    .GroupBy(x => x.Name)
                    .OrderBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Count())
            );
        _movieRepositoryMock
            .Setup(x => x.GetPremiereYears())
            .Returns(movies.Select(x => x.PremiereDate));
        _movieRepositoryMock
            .Setup(x => x.GetGenreCount())
            .ReturnsAsync(movies.SelectMany(x => x.Genres).Select(x => x.Name).Distinct().Count());
        _movieRepositoryMock
            .Setup(x => x.GetHighestRatedMedia(5))
            .ReturnsAsync(movies.OrderByDescending(x => x.CommunityRating));
        _movieRepositoryMock
            .Setup(x => x.GetLowestRatedMedia(5))
            .ReturnsAsync(movies.Where(x => x.CommunityRating != null).OrderBy(x => x.CommunityRating));
        _movieRepositoryMock
            .Setup(x => x.GetLatestAddedMedia(5))
            .Returns(movies.OrderByDescending(x => x.DateCreated));
        _movieRepositoryMock
            .Setup(x => x.GetNewestPremieredMedia(5))
            .ReturnsAsync(movies.OrderByDescending(x => x.PremiereDate));
        _movieRepositoryMock
            .Setup(x => x.GetOldestPremieredMedia(5))
            .ReturnsAsync(movies.OrderBy(x => x.PremiereDate));
        _movieRepositoryMock
            .Setup(x => x.GetLongestMovie(5))
            .Returns(movies.OrderByDescending(x => x.RunTimeTicks));
        _movieRepositoryMock
            .Setup(x => x.GetShortestMovie(It.IsAny<long>(), 5))
            .Returns(movies.OrderBy(x => x.RunTimeTicks));
        _movieRepositoryMock
            .Setup(x => x.GetTotalDiskSpace())
            .Returns(movies.Sum(x => x.MediaSources.FirstOrDefault()?.SizeInMb ?? 0));
        _movieRepositoryMock
            .Setup(x => x.GetTotalRuntime())
            .Returns(movies.Sum(x => x.RunTimeTicks ?? 0));
        _movieRepositoryMock
            .Setup(x => x.GetToShortMovieList(10))
            .Returns(movies.Where(x => x.RunTimeTicks < new TimeSpan(0, 0, 10, 0).Ticks).ToList);
        _movieRepositoryMock
            .Setup(x => x.GetPeopleCount(PersonType.Actor))
            .Returns(movies.SelectMany(x => x.People).DistinctBy(x => x.Id).Count(x => x.Type == PersonType.Actor));
        _movieRepositoryMock
            .Setup(x => x.GetPeopleCount(PersonType.Writer))
            .Returns(movies.SelectMany(x => x.People).DistinctBy(x => x.Id)
                .Count(x => x.Type == PersonType.Writer));
        _movieRepositoryMock
            .Setup(x => x.GetPeopleCount(PersonType.Director))
            .Returns(movies.SelectMany(x => x.People).DistinctBy(x => x.Id)
                .Count(x => x.Type == PersonType.Director));
        _movieRepositoryMock
            .Setup(x => x.Count())
            .ReturnsAsync(movies.Length);
        _movieRepositoryMock
            .Setup(x => x.GetMoviesWithoutPrimaryImage())
            .Returns(movies.Where(x => string.IsNullOrEmpty(x.Primary)));
        _movieRepositoryMock
            .Setup(x => x.GetMoviesWithoutImdbId())
            .Returns(movies.Where(x => string.IsNullOrEmpty(x.IMDB)));
        _movieRepositoryMock
            .Setup(x => x.GetMoviePage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Filter[]>()))
            .ReturnsAsync(movies.Where(x => x.Id == _movieOne.Id));
        _movieRepositoryMock
            .Setup(x => x.Count(It.IsAny<Filter[]>()))
            .ReturnsAsync(11);
        _movieRepositoryMock
            .Setup(x => x.GetById(It.IsAny<string>()))
            .ReturnsAsync(_movieOne);            
            
        _mediaServerRepositoryMock.Setup(x => x.GetAllLibraries(It.IsAny<LibraryType>()))
            .ReturnsAsync(new List<Library>
            {
                new LibraryBuilder(0, LibraryType.Movies).Build(),
                new LibraryBuilder(1, LibraryType.Movies).Build(),
            });
            
        var statisticsRepositoryMock = new Mock<IStatisticsRepository>();
        var jobRepositoryMock = new Mock<IJobRepository>();
        return new MovieService(_movieRepositoryMock.Object, settingsServiceMock.Object, 
            statisticsRepositoryMock.Object, jobRepositoryMock.Object, _mediaServerRepositoryMock.Object, _logger.Object);
    }

    #region General

    [Fact]
    public async Task GetMoviePage_Should_Return_Page_With_Total_Count()
    {
        var filters = Array.Empty<Filter>();
        var result = await _subject.GetMoviePage(0, 1, "name", "asc", filters, true);
        result.Should().NotBeNull();

        result.TotalCount.Should().Be(11);
        var results = result.Data.ToList();
        results.Count.Should().Be(1);
        results[0].Id.Should().Be(_movieOne.Id);
            
        _movieRepositoryMock.Verify(x => x.GetMoviePage(0, 1, "name", "asc", filters));
        _movieRepositoryMock.Verify(x => x.Count(filters));
        _movieRepositoryMock.VerifyNoOtherCalls();
            
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetMoviePage_Should_Return_Page_WithoutTotal_Count()
    {
        var filters = Array.Empty<Filter>();
        var result = await _subject.GetMoviePage(0, 1, "name", "asc", filters, false);
        result.Should().NotBeNull();

        result.TotalCount.Should().Be(0);
        var results = result.Data.ToList();
        results.Count.Should().Be(1);
        results[0].Id.Should().Be(_movieOne.Id);
            
        _movieRepositoryMock.Verify(x => x.GetMoviePage(0, 1, "name", "asc", filters));
        _movieRepositoryMock.VerifyNoOtherCalls();
            
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovie_Returns_Movie()
    {
        var movie = await _subject.GetMovie(_movieOne.Id);

        movie.Should().NotBeNull();
        movie.Id.Should().Be(_movieOne.Id);
            
        _movieRepositoryMock.Verify(x => x.GetById(_movieOne.Id));
        _movieRepositoryMock.VerifyNoOtherCalls();
            
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task SetLibraryAsSynced_Should_Update_Libraries()
    {
        var list = new[] {"1", "2"};
        await _subject.SetLibraryAsSynced(list);

        _mediaServerRepositoryMock.Verify(x => x.SetLibraryAsSynced(list, LibraryType.Movies));
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
            
        _movieRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task Get_Collections_From_Database()
    {
        var collections = await _subject.GetMovieLibraries();

        collections.Should().NotBeNull();
        collections.Count.Should().Be(2);
            
        _mediaServerRepositoryMock.Verify(x => x.GetAllLibraries(LibraryType.Movies));
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
            
        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_MovieCountStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Cards.Should().NotBeNull();
        stat.Cards.Count(x => x.Title == Constants.Movies.TotalMovies).Should().Be(1);

        var card = stat.Cards.First(x => x.Title == Constants.Movies.TotalMovies);
        card.Title.Should().Be(Constants.Movies.TotalMovies);
        card.Value.Should().Be("3");
    }

    [Fact]
    public async Task Get_GenreCountStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Cards.Should().NotBeNull();
        stat.Cards.Count(x => x.Title == Constants.Common.TotalGenres).Should().Be(1);

        var card = stat.Cards.First(x => x.Title == Constants.Common.TotalGenres);
        card.Title.Should().Be(Constants.Common.TotalGenres);
        card.Value.Should().Be("3");
    }

    [Fact]
    public async Task Get_LowestRatedStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Movies.LowestRated).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Movies.LowestRated);
        card.Title.Should().Be(Constants.Movies.LowestRated);
        card.Unit.Should().Be("/10");
        card.Values[0].Value.Should().Be(_movieOne.CommunityRating.ToString());
        card.Values[0].Label.Should().Be(_movieOne.Name);
        card.UnitNeedsTranslation.Should().Be(false);
        card.ValueType.Should().Be(ValueType.None);
    }

    [Fact]
    public async Task Get_HighestRatedStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Movies.HighestRated).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Movies.HighestRated);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Movies.HighestRated);
        card.Unit.Should().Be("/10");
        card.Values[0].Value.Should().Be(_movieThree.CommunityRating.ToString());
        card.Values[0].Label.Should().Be(_movieThree.Name);
        card.UnitNeedsTranslation.Should().Be(false);
        card.ValueType.Should().Be(ValueType.None);
    }

    [Fact]
    public async Task Get_OldestPremieredStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Movies.OldestPremiered).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Movies.OldestPremiered);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Movies.OldestPremiered);
        card.Unit.Should().Be("COMMON.DATE");
        card.Values[0].Value.Should().Be(_movieOne.PremiereDate?.ToString("s"));
        card.Values[0].Label.Should().Be(_movieOne.Name);
        card.UnitNeedsTranslation.Should().Be(true);
        card.ValueType.Should().Be(ValueType.Date);
    }

    [Fact]
    public async Task Get_NewestPremieredStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Movies.NewestPremiered).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Movies.NewestPremiered);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Movies.NewestPremiered);
        card.Unit.Should().Be("COMMON.DATE");
        card.Values[0].Value.Should().Be(_movieThree.PremiereDate?.ToString("s"));
        card.Values[0].Label.Should().Be(_movieThree.Name);
        card.UnitNeedsTranslation.Should().Be(true);
        card.ValueType.Should().Be(ValueType.Date);
    }

    [Fact]
    public async Task Get_ShortestStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Movies.Shortest).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Movies.Shortest);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Movies.Shortest);
        card.Unit.Should().Be("COMMON.MIN");
        card.Values[0].Value.Should().Be(_movieOne.RunTimeTicks.ToString());
        card.Values[0].Label.Should().Be(_movieOne.Name);
        card.UnitNeedsTranslation.Should().Be(true);
        card.ValueType.Should().Be(ValueType.Ticks);
    }

    [Fact]
    public async Task Get_LongestStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Movies.Longest).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Movies.Longest);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Movies.Longest);
        card.Unit.Should().Be("COMMON.MIN");
        card.Values[0].Value.Should().Be(_movieThree.RunTimeTicks.ToString());
        card.Values[0].Label.Should().Be(_movieThree.Name);
        card.UnitNeedsTranslation.Should().Be(true);
        card.ValueType.Should().Be(ValueType.Ticks);
    }

    [Fact]
    public async Task Get_LatestAddedStat()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.TopCards.Count(x => x.Title == Constants.Movies.LatestAdded).Should().Be(1);

        var card = stat.TopCards.First(x => x.Title == Constants.Movies.LatestAdded);
        card.Should().NotBeNull();
        card.Title.Should().Be(Constants.Movies.LatestAdded);
        card.Unit.Should().Be("COMMON.DATE");
        card.Values[0].Value.Should().Be(_movieOne.DateCreated?.ToString("s"));
        card.Values[0].Label.Should().Be(_movieOne.Name);
        card.UnitNeedsTranslation.Should().Be(true);
        card.ValueType.Should().Be(ValueType.Date);
    }

    [Fact]
    public async Task Get_TotalDiskSize()
    {
        var stat = await _subject.GetStatistics();

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
    public async Task Get_TotalPlayLengthStat()
    {
        var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(56, 34, 1).Build();
        var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
        var stat = await service.GetStatistics();
        stat.Cards.Count(x => x.Title == Constants.Movies.TotalPlayLength).Should().Be(1);

        var card = stat.Cards.First(x => x.Title == Constants.Movies.TotalPlayLength);
        card.Title.Should().Be(Constants.Movies.TotalPlayLength);
        card.Value.Should().Be("2|18|4");
    }

    [Fact]
    public async Task Calculate_GenreChart()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Charts.Count.Should().Be(4);
        stat.Charts.Any(x => x.Title == Constants.CountPerGenre).Should().BeTrue();

        var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerGenre);
        graph.Should().NotBeNull();
        graph!.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(3);
        graph.DataSets[0].Label.Should().Be("Action");
        graph.DataSets[0].Value.Should().Be(2);
        graph.DataSets[1].Label.Should().Be("Comedy");
        graph.DataSets[1].Value.Should().Be(2);
        graph.DataSets[2].Label.Should().Be("Drama");
        graph.DataSets[2].Value.Should().Be(1);
    }

    [Fact]
    public async Task Calculate_OfficialRatingChart()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Charts.Count.Should().Be(4);
        stat.Charts.Any(x => x.Title == Constants.CountPerOfficialRating).Should().BeTrue();

        var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerOfficialRating);
        graph.Should().NotBeNull();
        graph!.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(2);
        graph.DataSets[0].Label.Should().Be("R");
        graph.DataSets[0].Value.Should().Be(2);
        graph.DataSets[1].Label.Should().Be("B");
        graph.DataSets[1].Value.Should().Be(1);
    }

    [Fact]
    public async Task Calculate_OfficialRatingChart_Without_Movies()
    {
        var service = CreateMovieService(_settingsServiceMock);
        var stat = await service.GetStatistics();

        stat.Should().NotBeNull();
        stat.Charts.Count.Should().Be(4);
        stat.Charts.Any(x => x.Title == Constants.CountPerOfficialRating).Should().BeTrue();

        var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerOfficialRating);
        graph.Should().NotBeNull();
        graph!.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(0);
    }

    [Fact]
    public async Task Calculate_RatingChart()
    {
        var stat = await _subject.GetStatistics();

        stat.Should().NotBeNull();
        stat.Charts.Count.Should().Be(4);
        stat.Charts.Any(x => x.Title == Constants.CountPerCommunityRating).Should().BeTrue();

        var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerCommunityRating);
        graph.Should().NotBeNull();
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
    }

    [Fact]
    public async Task Calculate_RatingChart_Without_Movies()
    {
        var service = CreateMovieService(_settingsServiceMock);
        var stat = await service.GetStatistics();

        stat.Should().NotBeNull();
        stat.Charts.Count.Should().Be(4);
        stat.Charts.Any(x => x.Title == Constants.CountPerCommunityRating).Should().BeTrue();

        var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerCommunityRating);
        graph.Should().NotBeNull();
        graph!.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(20);
        graph.DataSets[0].Label.Should().Be("0");
        graph.DataSets[0].Value.Should().Be(0);
        graph.DataSets[1].Label.Should().Be(0.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[1].Value.Should().Be(0);
        graph.DataSets[2].Label.Should().Be("1");
        graph.DataSets[2].Value.Should().Be(0);
        graph.DataSets[3].Label.Should().Be(1.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[3].Value.Should().Be(0);
        graph.DataSets[4].Label.Should().Be("2");
        graph.DataSets[4].Value.Should().Be(0);
        graph.DataSets[5].Label.Should().Be(2.5.ToString(CultureInfo.CurrentCulture));
        graph.DataSets[5].Value.Should().Be(0);
        graph.DataSets[6].Label.Should().Be("3");
        graph.DataSets[6].Value.Should().Be(0);
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
    }

    [Fact]
    public async Task Calculate_PremiereYearChart()
    {
        var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1991, 3, 12))
            .Build();
        var movieFive = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1992, 3, 12))
            .Build();
        var movieSix = new MovieBuilder(Guid.NewGuid().ToString()).AddPremiereDate(new DateTime(1989, 3, 12))
            .Build();
        var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour,
            movieFive, movieSix);

        var stat = await service.GetStatistics();
        stat.Should().NotBeNull();
        stat.Charts.Count.Should().Be(4);
        stat.Charts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

        var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerPremiereYear);
        graph.Should().NotBeNull();
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
    }

    [Fact]
    public async Task Calculate_PremiereYearChart_Without_Movies()
    {
        var service = CreateMovieService(_settingsServiceMock);

        var stat = await service.GetStatistics();
        stat.Should().NotBeNull();
        stat.Charts.Count.Should().Be(4);
        stat.Charts.Any(x => x.Title == Constants.CountPerPremiereYear).Should().BeTrue();

        var graph = stat.Charts.SingleOrDefault(x => x.Title == Constants.CountPerPremiereYear);
        graph.Should().NotBeNull();
        graph!.SeriesCount.Should().Be(1);
        graph.DataSets.Length.Should().Be(0);
    }

    #endregion

    #region Suspicious

    // TODO: fix after implementation
    // [Fact]
    // public async Task ShortMovies()
    // {
    //     var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(0, 1, 0).Build();
    //     var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
    //     var stat = await service.GetStatistics();
    //
    //     stat.Should().NotBeNull();
    //     stat.Shorts.Should().NotBeNull();
    //     stat.Shorts.Count().Should().Be(1);
    //     var shortMovie = stat.Shorts.Single();
    //
    //     shortMovie.Title = movieFour.Name;
    //     shortMovie.MediaId = movieFour.Id;
    //     shortMovie.Number = 0;
    //     shortMovie.Duration = 60;
    // }
    //
    // [Fact]
    // public async Task ShortMoviesWithSettingDisabled()
    // {
    //     var settingsServiceMock = new Mock<ISettingsService>();
    //     settingsServiceMock
    //         .Setup(x => x.GetUserSettings())
    //         .Returns(new UserSettings
    //         {
    //             ToShortMovie = 10,
    //             ToShortMovieEnabled = false
    //         });
    //     var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddRunTimeTicks(0, 1, 0).Build();
    //     var service = CreateMovieService(settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
    //     var stat = await service.GetStatistics();
    //
    //     stat.Should().NotBeNull();
    //     stat.Shorts.Should().NotBeNull();
    //     stat.Shorts.Count().Should().Be(0);
    // }
        
    [Fact]
    public async Task MoviesWithoutImdb()
    {
        var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddImdb(string.Empty).Build();
        var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
        var stat = await service.GetStatistics();
        
        stat.Should().NotBeNull();
        stat.NoImdb.Should().NotBeNull();
        stat.NoImdb.Count().Should().Be(1);
        var noImdbIdMovie = stat.NoImdb.Single();
        
        noImdbIdMovie.Title = movieFour.Name;
        noImdbIdMovie.MediaId = movieFour.Id;
        noImdbIdMovie.Number = 0;
    }
        
    [Fact]
    public async Task MoviesWithoutPrimaryImage()
    {
        var movieFour = new MovieBuilder(Guid.NewGuid().ToString()).AddPrimaryImage(string.Empty).Build();
        var service = CreateMovieService(_settingsServiceMock, _movieOne, _movieTwo, _movieThree, movieFour);
        var stat = await service.GetStatistics();
        
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