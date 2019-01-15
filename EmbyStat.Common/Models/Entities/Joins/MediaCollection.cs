using System;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities.Joins
{
    public class MediaCollection
    {
        [Key]
        public string Id { get; set; }
        public Guid MediaId { get; set; }
        public Media Media { get; set; }
        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
