using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models.Joins
{
    public class StatisticCollection
    {
        [Key]
        public string Id { get; set; }
        public string StatisticId { get; set; }
        public Statistic Statistic { get; set; }
        public string CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
