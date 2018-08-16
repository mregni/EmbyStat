using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EmbyStat.Common.Models.Helpers;
using EmbyStat.Common.Models.Joins;

namespace EmbyStat.Common.Models
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
