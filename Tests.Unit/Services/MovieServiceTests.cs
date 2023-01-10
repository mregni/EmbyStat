using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Statistics;
using EmbyStat.Common.Models.Query;
using EmbyStat.Configuration.Interfaces;
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
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly Mock<IMediaServerRepository> _mediaServerRepositoryMock;
    private readonly Mock<IStatisticsService> _statisticsService;

    public MovieServiceTests()
    {
        var actorIdOne = 1;

        _movieRepositoryMock = new Mock<IMovieRepository>();
        _mediaServerRepositoryMock = new Mock<IMediaServerRepository>();
        _statisticsService = new Mock<IStatisticsService>();
        
        _movieOne = new MovieBuilder(Guid.NewGuid().ToString())
            .AddCommunityRating(1.7M)
            .AddOfficialRating("R")
            .AddPremiereDate(new DateTime(2002, 4, 2, 0, 0, 0))
            .AddRunTimeTicks(2, 10, 0)
            .AddName("The lord of the rings")
            .AddSortName("The-lord-of-the-rings")
            .AddGenres(new Genre {Name = "Action"}, new Genre {Name = "Drama"})
            .Build();

        var movieTwo = new MovieBuilder(Guid.NewGuid().ToString())
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

        var movieThree = new MovieBuilder(Guid.NewGuid().ToString())
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

        var configurationServiceMock = new Mock<IConfigurationService>();
        _subject = CreateMovieService(configurationServiceMock, _movieOne, movieTwo, movieThree);
    }

    private MovieService CreateMovieService(Mock<IConfigurationService> configurationService, params Movie[] movies)
    {
        _movieRepositoryMock
            .Setup(x => x.Count(It.IsAny<Filter[]>()))
            .ReturnsAsync(11);
        _movieRepositoryMock
            .Setup(x => x.GetMoviePage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Filter[]>()))
            .ReturnsAsync(movies.Where(x => x.Id == _movieOne.Id));
        _movieRepositoryMock
            .Setup(x => x.GetById(It.IsAny<string>()))
            .ReturnsAsync(_movieOne);
        _movieRepositoryMock
            .Setup(x => x.Any())
            .Returns(true);
        _mediaServerRepositoryMock.Setup(x => x.GetAllLibraries(It.IsAny<LibraryType>()))
            .ReturnsAsync(new List<Library>
            {
                new LibraryBuilder(0, LibraryType.Movies).Build(),
                new LibraryBuilder(1, LibraryType.Movies).Build(),
            });
            
        return new MovieService(_movieRepositoryMock.Object, configurationService.Object, 
            _mediaServerRepositoryMock.Object, _statisticsService.Object);
    }

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
            
        _movieRepositoryMock.Verify(x => x.RemoveUnwantedMovies(list));
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
    public void TypeIsPresent_Should_Return_True()
    {
        var result = _subject.TypeIsPresent();
        result.Should().BeTrue();

        _movieRepositoryMock.Verify(x => x.Any(), Times.Once);
    }

    [Fact]
    public async Task GetStatistics_Should_Find_Statistics()
    {
        var configurationServiceMock = new Mock<IConfigurationService>();

        var id = Constants.StatisticPageIds.MoviePage;
        _statisticsService
            .Setup(x => x.GetPage(id))
            .ReturnsAsync(new StatisticPageBuilder().UseMovieCard(false).Build());
        
        var service = CreateMovieService(configurationServiceMock);

        var stats = await service.GetStatistics();
        stats.Should().NotBeNull();
        stats.Cards.Count().Should().Be(4);
        stats.TopCards.Count().Should().Be(2);
        stats.Charts.Count().Should().Be(2);
        stats.ComplexCharts.Count().Should().Be(2);
        
        _statisticsService.Verify(x => x.GetPage(id), Times.Once());
        _statisticsService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetStatistics_Should_Find_Statistics_And_Calculate_Live_Stats()
    {
        var configurationServiceMock = new Mock<IConfigurationService>();

        var id = Constants.StatisticPageIds.MoviePage;
        _statisticsService
            .Setup(x => x.GetPage(id))
            .ReturnsAsync(new StatisticPageBuilder().UseMovieCard(true).Build());
        
        var service = CreateMovieService(configurationServiceMock);

        var stats = await service.GetStatistics();
        stats.Should().NotBeNull();
        stats.Cards.Count().Should().Be(5);
        stats.TopCards.Count().Should().Be(2);
        stats.Charts.Count().Should().Be(2);
        stats.ComplexCharts.Count().Should().Be(2);
        
        _statisticsService.Verify(x => x.GetPage(id), Times.Once());
        _statisticsService.Verify(x => x.CalculateCard(It.IsAny<StatisticCard>()), Times.Once());
        _statisticsService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetStatistics_Should_Calculate_New_Statistics()
    {
        var configurationServiceMock = new Mock<IConfigurationService>();

        var id = Constants.StatisticPageIds.MoviePage;
        _statisticsService
            .Setup(x => x.GetPage(id))
            .ReturnsAsync((StatisticPage)null);
        _statisticsService
            .Setup(x => x.CalculatePage(id))
            .ReturnsAsync(new StatisticPageBuilder().UseMovieCard(false).Build());
        
        var service = CreateMovieService(configurationServiceMock);

        var stats = await service.GetStatistics();
        stats.Should().NotBeNull();
        stats.Cards.Count().Should().Be(4);
        stats.TopCards.Count().Should().Be(2);
        stats.Charts.Count().Should().Be(2);
        stats.ComplexCharts.Count().Should().Be(2);
        
        _statisticsService.Verify(x => x.GetPage(id), Times.Once());
        _statisticsService.Verify(x => x.CalculatePage(id), Times.Once());
        _statisticsService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetStatistics_Should_Throw_Error()
    {
        var configurationServiceMock = new Mock<IConfigurationService>();

        var id = Constants.StatisticPageIds.MoviePage;
        _statisticsService
            .Setup(x => x.GetPage(id))
            .ReturnsAsync((StatisticPage)null);
        _statisticsService
            .Setup(x => x.CalculatePage(id))
            .ReturnsAsync((StatisticPage)null);
        
        var service = CreateMovieService(configurationServiceMock);

        await service.Invoking(async y => await service.GetStatistics())
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Page {id} is not found");
        
        _statisticsService.Verify(x => x.GetPage(id), Times.Once());
        _statisticsService.Verify(x => x.CalculatePage(id), Times.Once());
        _statisticsService.VerifyNoOtherCalls();
    }
}