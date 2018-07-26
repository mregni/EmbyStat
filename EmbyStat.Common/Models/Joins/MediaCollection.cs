using System;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models.Joins
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
