using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities.Statistics;
using Newtonsoft.Json;

namespace Tests.Unit.Builders;

public class StatisticPageBuilder
{
    private readonly StatisticPage _page;

    public StatisticPageBuilder()
    {
        _page = new StatisticPage()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            PageCards = new List<StatisticPageCard>()
        };
    }

    public StatisticPageBuilder UseShowCard(bool extra)
    {
        _page.PageCards = new List<StatisticPageCard>
        {
            new () { Order = 1, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.Card, Statistic.ShowTotalCount ){ Data = JsonConvert.SerializeObject(new Card())}},
            new () { Order = 2, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.Card, Statistic.ShowTotalActorCount ){ Data = JsonConvert.SerializeObject(new Card())}},
            new () { Order = 3, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.Card, Statistic.ShowCompleteCollectedCount ){ Data = JsonConvert.SerializeObject(new Card())}},
            new () { Order = 4, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.Card, Statistic.ShowTotalGenreCount ){ Data = JsonConvert.SerializeObject(new Card())}},
            new () { Order = 1, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.TopCard, Statistic.ShowHighestRatedList ){ Data = JsonConvert.SerializeObject(new TopCard())}},
            new () { Order = 2, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.TopCard, Statistic.ShowLatestAddedList ){ Data = JsonConvert.SerializeObject(new TopCard())}},
            new () { Order = 1, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.BarChart, Statistic.ShowGenreChart ){ Data = JsonConvert.SerializeObject(new Chart())}},
            new () { Order = 2, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.BarChart, Statistic.ShowRatingChart ){ Data = JsonConvert.SerializeObject(new Chart())}},
            new () { Order = 1, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.PieChart, Statistic.ShowStateChart ){ Data = JsonConvert.SerializeObject(new Chart())}},
            new () { Order = 1, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.ComplexChart, Statistic.EpisodeWatchedPerHourOfDayChart ){ Data = JsonConvert.SerializeObject(new ComplexChart())}},
            new () { Order = 2, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.ComplexChart, Statistic.EpisodeWatchedPerDayOfWeekChart ){ Data = JsonConvert.SerializeObject(new ComplexChart())}},
        };
        
        if (extra)
        {
            _page.PageCards.Add(new () { Order = 5, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Show, StatisticCardType.Card, Statistic.ShowTotalCurrentWatchingCount ){ Data = JsonConvert.SerializeObject(new Card())}});
        }
        
        return this;
    }
    
    public StatisticPageBuilder UseMovieCard(bool extra)
    {
        _page.PageCards = new List<StatisticPageCard>
        {
            new () { Order = 1, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.Card, Statistic.MovieTotalCount ){ Data = JsonConvert.SerializeObject(new Card())}},
            new () { Order = 2, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.Card, Statistic.MovieTotalActorCount ){ Data = JsonConvert.SerializeObject(new Card())}},
            new () { Order = 3, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.Card, Statistic.MovieTotalDirectorCount ){ Data = JsonConvert.SerializeObject(new Card())}},
            new () { Order = 4, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.Card, Statistic.MovieTotalGenreCount ){ Data = JsonConvert.SerializeObject(new Card())}},
            new () { Order = 1, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.TopCard, Statistic.MovieHighestRatedList ){ Data = JsonConvert.SerializeObject(new TopCard())}},
            new () { Order = 2, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.TopCard, Statistic.MovieLatestAddedList ){ Data = JsonConvert.SerializeObject(new TopCard())}},
            new () { Order = 1, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.BarChart, Statistic.MovieGenreChart ){ Data = JsonConvert.SerializeObject(new Chart())}},
            new () { Order = 2, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.BarChart, Statistic.MovieRatingChart ){ Data = JsonConvert.SerializeObject(new Chart())}},
            new () { Order = 1, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.ComplexChart, Statistic.MovieWatchedPerHourOfDayChart ){ Data = JsonConvert.SerializeObject(new ComplexChart())}},
            new () { Order = 2, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.ComplexChart, Statistic.MovieWatchedPerDayOfWeekChart ){ Data = JsonConvert.SerializeObject(new ComplexChart())}},
        };

        if (extra)
        {
            _page.PageCards.Add(new () { Order = 5, StatisticCard = new StatisticCard(Guid.NewGuid(), StatisticType.Movie, StatisticCardType.Card, Statistic.MovieTotalCurrentWatchingCount ){ Data = JsonConvert.SerializeObject(new Card())}});
        }

        return this;
    }

    public StatisticPage Build()
    {
        return _page;
    }
}