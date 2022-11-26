using System.Globalization;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Jobs.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Core.Abstract;

public abstract class MediaService : StatisticHelper
{
    private readonly IConfigurationService _configurationService;
    protected MediaService(IJobRepository jobRepository, ILogger<MediaService> logger, IConfigurationService configurationService)
    :base(logger, jobRepository)
    {
        _configurationService = configurationService;
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

    internal Task<Card> GetCurrentWatchingCount(Func<Task<int>> action, string title)
    {
        return CalculateStat(async () =>
        {
            var count = await action();

            return new Card
            {
                Title = title,
                Value = $"{count}",
                Type = CardType.Text,
                Icon = Constants.Icons.PlayRoundedIcon
            };
        }, "Calculate now playing count failed:");
    }
    
    internal Task<MultiChart> CalculateWatchedPerHourOfDayChart(Func<Task<IEnumerable<BarValue<string, int>>>> action, string title)
    {
        var timeZone = _configurationService.GetLocalTimeZoneInfo();
        return CalculateStat(async () =>
        {
            var values = await action();
            var chart = new JArray();

            var list = values.ToList();
            foreach (var value in list.GroupBy(x => x.X))
            {
                var time = TimeZoneInfo.ConvertTimeFromUtc(new DateTime().AddHours(int.Parse(value.Key)), timeZone);
                var serie = new JObject(new JProperty("label", time));
                foreach (var barValue in value)
                {
                    serie.Add(new JProperty(barValue.Serie, barValue.Y));
                }
                chart.Add(serie);
            }
            
            for (var i = 0; i < 24; i += 1)
            {
                if (!chart.Any(x => x["label"]?.Value<DateTime>() == new DateTime().AddHours(i)))
                {
                    chart.Add(new JObject(new JProperty("label", new DateTime().AddHours(i))));
                }
            }

            var chartData = chart
                .OrderBy(x => x["label"].Value<DateTime>())
                .ToArray();

            return new MultiChart
            {
                Title = title,
                DataSets = JsonConvert.SerializeObject(chartData),
                FormatString = "p", // Format datetime to 23:00 or 11:00 PM
                Series = list
                    .GroupBy(x => x.Serie)
                    .Select(x => x.Key)
                    .Distinct()
                    .ToArray()
            };
        }, "Calculate movie views per hour chart failed:");
    }
    
    internal Task<MultiChart> CalculateWatchedPerDayOfWeekChart(Func<Task<IEnumerable<BarValue<string, int>>>> action, string title)
    {
        return CalculateStat(async () =>
        {
            var values = await action();
            var chart = new JArray();

            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var list = values.ToList();
            foreach (var value in list.GroupBy(x => x.X))
            {
                var serie = new JObject(new JProperty("label", value.Key));
                foreach (var barValue in value)
                {
                    serie.Add(new JProperty(barValue.Serie, barValue.Y));
                }
                chart.Add(serie);
            }
            
            for (var i = 0; i < 7; i += 1)
            {
                if (!chart.Any(x => x["label"]?.Value<int>() == i))
                {
                    chart.Add(new JObject(new JProperty("label", i.ToString())));
                }
            }

            var chartData = chart
                .OrderBy(x => x["label"].Value<int>())
                .ToList();

            while (chartData.First()["label"].Value<int>() != (int) firstDayOfWeek)
            {
                var tempSerie = chartData.First();
                chartData.RemoveAt(0);
                chartData.Add(tempSerie);
            }
            
            return new MultiChart
            {
                Title = title,
                DataSets = JsonConvert.SerializeObject(chartData),
                FormatString = "week",
                Series = list
                    .GroupBy(x => x.Serie)
                    .Select(x => x.Key)
                    .Distinct()
                    .ToArray()
            };
        }, "Calculate movie views per day of the week chart failed:");
    }

    #endregion
}