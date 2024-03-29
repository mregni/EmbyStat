﻿using EmbyStat.Common;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Core.Jobs.Interfaces;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Core.Abstract;

public abstract class MediaService : StatisticHelper
{
    protected MediaService(IJobRepository jobRepository, ILogger<MediaService> logger)
    :base(logger, jobRepository)
    {
        
    }

    #region Chart

    internal static Chart CreateGenreChart(Dictionary<string, int> items)
    {
        var genresData = items
            .Select(x => new SimpleChartData {Label = x.Key, Value = x.Value})
            .ToArray();

        return new Chart
        {
            Title = Constants.CountPerGenre,
            DataSets = genresData,
            SeriesCount = 1
        };
    }

    internal static Chart CreateRatingChart(IEnumerable<decimal?> items)
    {
        var ratingDataList = items.GroupBy(x => x.RoundToHalf())
            .OrderBy(x => x.Key)
            .ToList();

        for (double i = 0; i < 10; i += 0.5)
        {
            if (!ratingDataList.Any(x => x.Key == i))
            {
                ratingDataList.Add(new ChartGrouping<double?, decimal?> {Key = i, Capacity = 0});
            }
        }

        var ratingData = ratingDataList
            .Select(x => new SimpleChartData {Label = x.Key?.ToString() ?? Constants.Unknown, Value = x.Count()})
            .OrderBy(x => x.Label)
            .ToArray();

        return new Chart
        {
            Title = Constants.CountPerCommunityRating,
            DataSets = ratingData,
            SeriesCount = 1
        };
    }

    internal static Chart CalculatePremiereYearChart(IEnumerable<DateTime?> items)
    {
        var yearDataList = items.GroupBy(x => x.RoundToFiveYear())
            .Where(x => x.Key != null)
            .OrderBy(x => x.Key)
            .ToList();

        if (yearDataList.Any())
        {
            var lowestYear = yearDataList.Where(x => x.Key.HasValue).Min(x => x.Key);
            var highestYear = yearDataList.Where(x => x.Key.HasValue).Max(x => x.Key);

            for (var i = lowestYear; i < highestYear; i += 5)
            {
                if (yearDataList.All(x => x.Key != i))
                {
                    yearDataList.Add(new ChartGrouping<int?, DateTime?> {Key = i, Capacity = 0});
                }
            }
        }

        var yearData = yearDataList
            .Select(x => new SimpleChartData
                {Label = x.Key != null ? $"{x.Key} - {x.Key + 4}" : Constants.Unknown, Value = x.Count()})
            .OrderBy(x => x.Label)
            .ToArray();

        return new Chart
        {
            Title = Constants.CountPerPremiereYear,
            DataSets = yearData,
            SeriesCount = 1
        };
    }

    internal Chart CalculateOfficialRatingChart(Dictionary<string, int> items)
    {
        var ratingData = items
            .Select(x => new SimpleChartData {Label = x.Key, Value = x.Value})
            .ToArray();

        return new Chart
        {
            Title = Constants.CountPerOfficialRating,
            DataSets = ratingData,
            SeriesCount = 1
        };
    }

    #endregion
}