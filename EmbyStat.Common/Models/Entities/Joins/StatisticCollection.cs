using System;
using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities.Joins
{
    public class StatisticCollection
    {
        [Key]
        public Guid Id { get; set; }
        public Guid StatisticId { get; set; }
        public Statistic Statistic { get; set; }
        public string CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
