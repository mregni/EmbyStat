using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Joins;

namespace EmbyStat.Common.Models.Entities
{
    public class Statistic
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CalculationDateTime { get; set; }
        public StatisticType Type { get; set; }
        public string JsonResult { get; set; }
        public ICollection<StatisticCollection> Collections { get; set; }
        public bool IsValid { get; set; }
    }
}
