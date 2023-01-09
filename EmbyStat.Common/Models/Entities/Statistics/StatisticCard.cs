using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums.StatisticEnum;

namespace EmbyStat.Common.Models.Entities.Statistics;

public class StatisticCard
{
    public Guid Id { get; set; }
    public StatisticType Type { get; set; }
    public StatisticCardType CardType { get; set; }
    public Statistic UniqueType { get; set; }
    public ICollection<StatisticPageCard> PageCards { get; set; }
    public string Data { get; set; }

    public StatisticCard()
    {
        
    }

    public StatisticCard(Guid id, StatisticType type, StatisticCardType cardType, Statistic uniqueType)
    {
        Id = id;
        Type = type;
        CardType = cardType;
        UniqueType = uniqueType;
    }
}