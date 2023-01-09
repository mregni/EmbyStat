using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Statistics;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.Statistics.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Core.Statistics;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly EsDbContext _context;

    public StatisticsRepository(EsDbContext context)
    {
        _context = context;
    }

    public Task<StatisticOld> GetLastResultByType(StatisticType type)
    {
        return _context.Statistics
            .Where(x => x.IsValid && x.Type == type)
            .OrderByDescending(x => x.CalculationDateTime)
            .FirstOrDefaultAsync();
    }

    public async Task ReplaceStatistic(string json, DateTime calculationDateTime, StatisticType type)
    {
        _context.Statistics.RemoveRange(_context.Statistics.Where(x => x.Type == type));

        var statistic = new StatisticOld
        {
            CalculationDateTime = calculationDateTime,
            Type = type,
            JsonResult = json,
            IsValid = true
        };
        _context.Statistics.Add(statistic);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteStatistics()
    {
        _context.Statistics.RemoveRange(_context.Statistics.Where(x => !x.IsValid));
        await _context.SaveChangesAsync();
    }

    public async Task MarkTypesAsInvalid(StatisticType type)
    {
        var statistic = await _context.Statistics
            .FirstOrDefaultAsync(x => x.IsValid && x.Type == type);
        if (statistic != null)
        {
            statistic.IsValid = false;
            await _context.SaveChangesAsync();
        }
    }


    public Task<List<StatisticCard>> GetCardsByType(StatisticType type)
    {
        return _context.StatisticCards
            .Where(x => x.PageCards.Any(y => y.StatisticCard.Type == type))
            .ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task<StatisticPage?> GetPage(Guid id)
    {
        return _context.StatisticPages
            .Include(x => x.PageCards)
            .ThenInclude(x => x.StatisticCard)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}