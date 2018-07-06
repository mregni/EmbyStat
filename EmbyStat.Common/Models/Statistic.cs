using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models
{
    public class Statistic
    {
        [Key]
        public string Id { get; set; }
        public DateTime CalculationDateTime { get; set; }
        public StatisticType Type { get; set; }
        public string JsonResult { get; set; }
        public List<CollectionId> Collections { get; set; }
    }
}
