using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Entities.Statistics;
using EmbyStat.Core.Statistics;
using EmbyStat.Core.Statistics.Interfaces;
using FluentAssertions;
using Moq;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Services;

public class StatisticServiceTests
{
    private readonly Mock<IShowStatisticsService> _showStatisticsService;
    private readonly Mock<IMovieStatisticsService> _movieStatisticsService;
    private readonly Mock<IStatisticsRepository> _statisticsRepository;

    public StatisticServiceTests()
    {
        _statisticsRepository = new Mock<IStatisticsRepository>();
        _movieStatisticsService = new Mock<IMovieStatisticsService>();
        _showStatisticsService = new Mock<IShowStatisticsService>();
    }

    [Fact]
    public async Task CalculateStatisticsByType_Should_Calculate_Movie_Cards()
    {
        var cards = new List<StatisticCard>()
        {
            new(),
            new(),
            new()
        };
        _statisticsRepository
            .Setup(x => x.GetCardsByType(It.IsAny<StatisticType>()))
            .ReturnsAsync(cards);

        var service = new StatisticsService(_movieStatisticsService.Object, _showStatisticsService.Object,
            _statisticsRepository.Object);

        await service.CalculateStatisticsByType(StatisticType.Movie);

        _statisticsRepository.Verify(x => x.GetCardsByType(StatisticType.Movie), Times.Once);
        _statisticsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _statisticsRepository.VerifyNoOtherCalls();

        _showStatisticsService.VerifyNoOtherCalls();

        _movieStatisticsService.Verify(x => x.CalculateStatistic(It.IsAny<StatisticCardType>(), It.IsAny<Statistic>()),
            Times.Exactly(3));
        _movieStatisticsService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateStatisticsByType_Should_Calculate_Show_Cards()
    {
        var cards = new List<StatisticCard>
        {
            new() {Type = StatisticType.Show},
            new() {Type = StatisticType.Show},
            new() {Type = StatisticType.Show}
        };
        _statisticsRepository
            .Setup(x => x.GetCardsByType(It.IsAny<StatisticType>()))
            .ReturnsAsync(cards);

        var service = new StatisticsService(_movieStatisticsService.Object, _showStatisticsService.Object,
            _statisticsRepository.Object);

        await service.CalculateStatisticsByType(StatisticType.Show);

        _statisticsRepository.Verify(x => x.GetCardsByType(StatisticType.Show), Times.Once);
        _statisticsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _statisticsRepository.VerifyNoOtherCalls();

        _showStatisticsService.Verify(x => x.CalculateStatistic(It.IsAny<StatisticCardType>(), It.IsAny<Statistic>()),
            Times.Exactly(3));
        _showStatisticsService.VerifyNoOtherCalls();

        _movieStatisticsService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculatePage_Should_Calculate_Page()
    {
        var id = Guid.NewGuid();
        _statisticsRepository
            .Setup(x => x.GetPage(It.IsAny<Guid>()))
            .ReturnsAsync(new StatisticPageBuilder().UseShowCard(false).Build());

        var service = new StatisticsService(_movieStatisticsService.Object, _showStatisticsService.Object,
            _statisticsRepository.Object);

        var page = await service.CalculatePage(id);
        page.Should().NotBeNull();
        
        _showStatisticsService.Verify(x => x.CalculateStatistic(It.IsAny<StatisticCardType>(), It.IsAny<Statistic>()),
            Times.Exactly(11));
        _showStatisticsService.VerifyNoOtherCalls();
        
        _statisticsRepository.Verify(x => x.GetPage(id), Times.Once);
        _statisticsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _statisticsRepository.VerifyNoOtherCalls();

        _movieStatisticsService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task CalculatePage_Should_Return_Page()
    {
        var id = Guid.NewGuid();
        _statisticsRepository
            .Setup(x => x.GetPage(It.IsAny<Guid>()))
            .ReturnsAsync((StatisticPage)null);

        var service = new StatisticsService(_movieStatisticsService.Object, _showStatisticsService.Object,
            _statisticsRepository.Object);

        var page = await service.CalculatePage(id);
        page.Should().BeNull();
        
        _showStatisticsService.VerifyNoOtherCalls();
        
        _statisticsRepository.Verify(x => x.GetPage(id), Times.Once);
        _statisticsRepository.VerifyNoOtherCalls();

        _movieStatisticsService.VerifyNoOtherCalls();
    }
}