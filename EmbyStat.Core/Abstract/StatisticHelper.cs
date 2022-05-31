using EmbyStat.Common.Models.Entities;
using EmbyStat.Core.Jobs.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Core.Abstract;

public abstract class StatisticHelper
{
    private readonly ILogger<StatisticHelper> _logger;
    private readonly IJobRepository _jobRepository;
    public StatisticHelper(ILogger<StatisticHelper> logger, IJobRepository jobRepository)
    {
        _logger = logger;
        _jobRepository = jobRepository;
    }

    internal T CalculateStat<T>(Func<T> action, string errorMessage)
    {
        try
        {
            return action();
        }
        catch (Exception e)
        {
            _logger.LogError(e, errorMessage);
        }

        return default;
    }
    
    internal bool StatisticsAreValid(Statistic statistic, Guid jobId)
    {
        var lastMediaSync = _jobRepository.GetById(jobId);

        //TODO We need to add 5 minutes here because the CalculationDateTime is ALWAYS just in front of the EndTimeUtc
        //(Ugly fix for now)
        return statistic != null
               && lastMediaSync != null
               && statistic.CalculationDateTime.AddMinutes(5) > lastMediaSync.EndTimeUtc;
    }
}