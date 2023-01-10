using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Statistics;
using EmbyStat.Common.Models.Query;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Jobs.Interfaces;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Shows;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Unit.Builders;
using Xunit;
using ValueType = EmbyStat.Common.Models.Cards.ValueType;

namespace Tests.Unit.Services;

public class ShowServiceTests
{
    private readonly ShowService _subject;

    private readonly Show _showOne;

    private readonly Mock<IShowRepository> _showRepositoryMock;
    private readonly Mock<IMediaServerRepository> _mediaServerRepositoryMock;
    private readonly Mock<IStatisticsService> _statisticsService;

    public ShowServiceTests()
    {
        _showRepositoryMock = new Mock<IShowRepository>();
        _mediaServerRepositoryMock = new Mock<IMediaServerRepository>();
        _statisticsService = new Mock<IStatisticsService>();

        var showOneId = Guid.NewGuid().ToString();
        var showTwoId = Guid.NewGuid().ToString();
        var showThreeId = Guid.NewGuid().ToString();

        _showOne = new ShowBuilder(showOneId)
            .AddName("Chuck")
            .AddCreateDate(new DateTime(1990, 4, 2))
            .AddGenre("Comedy", "Action")
            .AddCommunityRating(null)
            .Build();
        var showTwo = new ShowBuilder(showTwoId)
            .AddName("The 100")
            .AddCommunityRating(8.3M)
            .AddPremiereDate(new DateTime(1992, 4, 1))
            .AddGenre("Drama", "Comedy", "Action")
            .SetContinuing()
            .AddOfficialRating("TV-16")
            .AddActor(_showOne.People.First().Id.ToString())
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddMissingEpisodes(10, 1)
            .Build();
        var showThree = new ShowBuilder(showThreeId)
            .AddName("Dexter")
            .AddCommunityRating(8.4M)
            .AddPremiereDate(new DateTime(2018, 4, 10))
            .AddCreateDate(new DateTime(2003, 4, 2))
            .AddGenre("War", "Action")
            .SetContinuing()
            .AddEpisode(new EpisodeBuilder(Guid.NewGuid().ToString(), "1").Build())
            .AddMissingEpisodes(2, 1)
            .Build();

        _subject = CreateShowService(_showOne, showTwo, showThree);
    }

    private ShowService CreateShowService(params Show[] shows)
    {
        _showRepositoryMock
            .Setup(x => x.Any())
            .Returns(true);
        _showRepositoryMock
            .Setup(x => x.GetShowPage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Filter[]>()))
            .ReturnsAsync(new[] {_showOne});
        _showRepositoryMock
            .Setup(x => x.Count(It.IsAny<Filter[]>()))
            .ReturnsAsync(11);
        _showRepositoryMock
            .Setup(x => x.GetShowByIdWithEpisodes(It.IsAny<string>()))
            .ReturnsAsync(_showOne);
        
        _mediaServerRepositoryMock.Setup(x => x.GetAllLibraries(It.IsAny<LibraryType>()))
            .ReturnsAsync(new List<Library>
            {
                new LibraryBuilder(0, LibraryType.TvShow).Build(),
                new LibraryBuilder(1, LibraryType.TvShow).Build(),
            });
        
        return new ShowService(_showRepositoryMock.Object, _mediaServerRepositoryMock.Object, _statisticsService.Object);
                
    }

    [Fact]
    public async Task GetCollectionsFromDatabase()
    {
        var collections = await _subject.GetShowLibraries();

        collections.Should().NotBeNull();
        collections.Count.Should().Be(2);
            
        _mediaServerRepositoryMock.Verify(x => x.GetAllLibraries(LibraryType.TvShow));
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
            
        _showRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetShow_Should_Return_Show()
    {
        var show = await _subject.GetShow(_showOne.Id);

        show.Should().NotBeNull();
        show.Id.Should().Be(_showOne.Id);
            
        _showRepositoryMock.Verify(x => x.GetShowByIdWithEpisodes(_showOne.Id));
        _showRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task SetLibraryAsSynced_Should_Update_Libraries()
    {
        var list = new[] {"1", "2"};
        await _subject.SetLibraryAsSynced(list);

        _mediaServerRepositoryMock.Verify(x => x.SetLibraryAsSynced(list, LibraryType.TvShow));
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
            
        _showRepositoryMock.Verify(x => x.RemoveUnwantedShows(list));
        _showRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetShowPage_Should_Return_Page_With_Total_Count()
    {
        var filters = Array.Empty<Filter>();
        var result = await _subject.GetShowPage(0, 1, "name", "asc", filters, true);
        result.Should().NotBeNull();

        result.TotalCount.Should().Be(11);
        var results = result.Data.ToList();
        results.Count.Should().Be(1);
        results[0].Id.Should().Be(_showOne.Id);
            
        _showRepositoryMock.Verify(x => x.GetShowPage(0, 1, "name", "asc", filters));
        _showRepositoryMock.Verify(x => x.Count(filters));
        _showRepositoryMock.VerifyNoOtherCalls();
            
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
    }
        
    [Fact]
    public async Task GetShowPage_Should_Return_Page_WithoutTotal_Count()
    {
        var filters = Array.Empty<Filter>();
        var result = await _subject.GetShowPage(0, 1, "name", "asc", filters, false);
        result.Should().NotBeNull();

        result.TotalCount.Should().Be(0);
        var results = result.Data.ToList();
        results.Count.Should().Be(1);
        results[0].Id.Should().Be(_showOne.Id);
            
        _showRepositoryMock.Verify(x => x.GetShowPage(0, 1, "name", "asc", filters));
        _showRepositoryMock.VerifyNoOtherCalls();
            
        _mediaServerRepositoryMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void TypeIsPresent_Should_Return_True()
    {
        var result = _subject.TypeIsPresent();
        result.Should().BeTrue();

        _showRepositoryMock.Verify(x => x.Any(), Times.Once);
    }
    
        [Fact]
    public async Task GetStatistics_Should_Find_Statistics()
    {
        var id = Constants.StatisticPageIds.ShowPage;
        _statisticsService
            .Setup(x => x.GetPage(id))
            .ReturnsAsync(new StatisticPageBuilder().UseShowCard(false).Build());
        
        var service = CreateShowService();
        var stats = await service.GetStatistics();
        stats.Should().NotBeNull();
        stats.Cards.Count().Should().Be(4);
        stats.TopCards.Count().Should().Be(2);
        stats.BarCharts.Count().Should().Be(2);
        stats.PieCharts.Count().Should().Be(1);
        stats.ComplexCharts.Count().Should().Be(2);
        
        _statisticsService.Verify(x => x.GetPage(id), Times.Once());
        _statisticsService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetStatistics_Should_Find_Statistics_And_Calculate_Live_Stats()
    {
        var id = Constants.StatisticPageIds.ShowPage;
        _statisticsService
            .Setup(x => x.GetPage(id))
            .ReturnsAsync(new StatisticPageBuilder().UseShowCard(true).Build());
        
        var service = CreateShowService();

        var stats = await service.GetStatistics();
        stats.Should().NotBeNull();
        stats.Cards.Count().Should().Be(5);
        stats.TopCards.Count().Should().Be(2);
        stats.BarCharts.Count().Should().Be(2);
        stats.PieCharts.Count().Should().Be(1);
        stats.ComplexCharts.Count().Should().Be(2);
        
        _statisticsService.Verify(x => x.GetPage(id), Times.Once());
        _statisticsService.Verify(x => x.CalculateCard(It.IsAny<StatisticCard>()), Times.Once());
        _statisticsService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetStatistics_Should_Calculate_New_Statistics()
    {
        var id = Constants.StatisticPageIds.ShowPage;
        _statisticsService
            .Setup(x => x.GetPage(id))
            .ReturnsAsync((StatisticPage)null);
        _statisticsService
            .Setup(x => x.CalculatePage(id))
            .ReturnsAsync(new StatisticPageBuilder().UseShowCard(false).Build());
        
        var service = CreateShowService();
        var stats = await service.GetStatistics();
        stats.Should().NotBeNull();
        stats.Cards.Count().Should().Be(4);
        stats.TopCards.Count().Should().Be(2);
        stats.BarCharts.Count().Should().Be(2);
        stats.PieCharts.Count().Should().Be(1);
        stats.ComplexCharts.Count().Should().Be(2);
        
        _statisticsService.Verify(x => x.GetPage(id), Times.Once());
        _statisticsService.Verify(x => x.CalculatePage(id), Times.Once());
        _statisticsService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetStatistics_Should_Throw_Error()
    {
        var id = Constants.StatisticPageIds.ShowPage;
        _statisticsService
            .Setup(x => x.GetPage(id))
            .ReturnsAsync((StatisticPage)null);
        _statisticsService
            .Setup(x => x.CalculatePage(id))
            .ReturnsAsync((StatisticPage)null);
        
        var service = CreateShowService();
        await service.Invoking(async y => await service.GetStatistics())
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Page {id} is not found");
        
        _statisticsService.Verify(x => x.GetPage(id), Times.Once());
        _statisticsService.Verify(x => x.CalculatePage(id), Times.Once());
        _statisticsService.VerifyNoOtherCalls();
    }
}