using System;
using EmbyStat.Common.Enums.StatisticEnum;

namespace EmbyStat.Common.Models.Entities;

[Obsolete("Will be replaced with StatisticPage table")]
public class StatisticOld
{
    public Guid Id { get; set; }
    public DateTime CalculationDateTime { get; set; }
    public StatisticType Type { get; set; }
    public string JsonResult { get; set; }
    public bool IsValid { get; set; }
}