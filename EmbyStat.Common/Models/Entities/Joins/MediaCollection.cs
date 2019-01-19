using System;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities.Joins
{
    public class MediaCollection
    {
        [Key]
        public Guid Id { get; set; }
        public string MediaId { get; set; }
        public Media Media { get; set; }
        public string CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
