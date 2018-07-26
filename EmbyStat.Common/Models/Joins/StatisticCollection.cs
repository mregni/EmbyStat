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
        public Guid Id { get; set; }
        public Guid StatisticId { get; set; }
        public Statistic Statistic { get; set; }
        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
