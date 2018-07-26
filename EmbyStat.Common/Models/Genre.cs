using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Joins;

namespace EmbyStat.Common.Models
{
    public class Genre
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<MediaGenre> MediaGenres { get; set; }
    }
}
