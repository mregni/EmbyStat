using System;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories;

public class SqlStatisticsRepository : IStatisticsRepository
{
    private readonly EsDbContext _context;

    public SqlStatisticsRepository(EsDbContext context)
    {
        _context = context;
    }

    public Task<Statistic> GetLastResultByType(StatisticType type)
    {
        return _context.Statistics
            .Where(x => x.IsValid && x.Type == type)
            .OrderByDescending(x => x.CalculationDateTime)
            .FirstOrDefaultAsync();
    }

    public async Task ReplaceStatistic(string json, DateTime calculationDateTime, StatisticType type)
    {
        _context.Statistics.RemoveRange(_context.Statistics.Where(x => x.Type == type));

        var statistic = new Statistic
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
}