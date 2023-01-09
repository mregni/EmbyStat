using System;
using System.Collections.Generic;

namespace EmbyStat.Common.Models.Entities.Statistics;

public class StatisticPage
{
    public Guid Id { get; set; }
    public DateTime? CalculationDateTime { get; set; }
    public string Name { get; set; }
    public ICollection<StatisticPageCard> PageCards { get; set; }

}