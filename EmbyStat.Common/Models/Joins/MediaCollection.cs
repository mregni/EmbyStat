using System;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models.Joins
{
    public class MediaCollection
    {
        [Key]
        public string Id { get; set; }
        public string MediaId { get; set; }
        public Media Media { get; set; }
        public string CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
