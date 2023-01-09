using System;

namespace EmbyStat.Common.Models.Entities.Statistics;

public class StatisticPageCard
{
    public int Order { get; set; }
    public StatisticPage StatisticPage { get; set; }
    public Guid StatisticPageId { get; set; }
    public StatisticCard StatisticCard { get; set; }
    public Guid StatisticCardId { get; set; }

    public StatisticPageCard()
    {
        
    }

    public StatisticPageCard(Guid statisticPageId, Guid statisticCardId, int order)
    {
        StatisticPageId = statisticPageId;
        StatisticCardId = statisticCardId;
        Order = order;
    }
}