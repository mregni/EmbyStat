using Microsoft.Extensions.Logging;

namespace EmbyStat.Core.Abstract;

public abstract class StatisticHelper
{
    private readonly ILogger<StatisticHelper> _logger;
    public StatisticHelper(ILogger<StatisticHelper> logger)
    {
        _logger = logger;
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

        return default!;
    }
}