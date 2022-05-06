using System;
using EmbyStat.Common.Enums;

namespace EmbyStat.Common.Models.Entities;

public class Statistic
{
    public Guid Id { get; set; }
    public DateTime CalculationDateTime { get; set; }
    public StatisticType Type { get; set; }
    public string JsonResult { get; set; }
    public bool IsValid { get; set; }
}