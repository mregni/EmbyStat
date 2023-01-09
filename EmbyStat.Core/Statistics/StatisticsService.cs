using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Entities.Statistics;
using EmbyStat.Core.Statistics.Interfaces;

namespace EmbyStat.Core.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly IMovieStatisticsService _movieStatisticsService;
    private readonly IShowStatisticsService _showStatisticsService;
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsService(IMovieStatisticsService movieStatisticsService,
        IShowStatisticsService showStatisticsService, IStatisticsRepository statisticsRepository)
    {
        _movieStatisticsService = movieStatisticsService;
        _showStatisticsService = showStatisticsService;
        _statisticsRepository = statisticsRepository;
    }

    public async Task CalculateStatisticsByType(StatisticType type)
    {
        var cards = await _statisticsRepository.GetCardsByType(type);
        foreach (var card in cards)
        {
            await CalculateStatistic(card);
        }

        await _statisticsRepository.SaveChangesAsync();
    }

    public async Task<StatisticPage?> CalculatePage(Guid id)
    {
        var page = await _statisticsRepository.GetPage(id);
        if (page != null)
        {
            foreach (var pageCard in page.PageCards)
            {
                await CalculateStatistic(pageCard.StatisticCard);
            }
            await _statisticsRepository.SaveChangesAsync();
        }

        return page;
    }

    public Task<StatisticPage?> GetPage(Guid id)
    {
        return _statisticsRepository.GetPage(id);
    }

    private async Task CalculateStatistic(StatisticCard card)
    {
        string? data = null;
        switch (card.Type)
        {
            case StatisticType.Movie:
                data = await _movieStatisticsService.CalculateStatistic(card.CardType, card.UniqueType);
                break;
            case StatisticType.Show:
                data = await _showStatisticsService.CalculateStatistic(card.CardType, card.UniqueType);
                break;
            case StatisticType.User:
            default:
                break;
        }

        card.Data = data;
    }
}