using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmbyStat.Common.Models
{
    public class Collection
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string PrimaryImage { get; set; }
        public CollectionType Type { get; set; }
    }

    public enum CollectionType
    {
        Other = 0,
        Movies = 1,
        TvShow = 2
    }
}
