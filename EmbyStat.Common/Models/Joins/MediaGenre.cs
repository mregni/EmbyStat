using System;
using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models.Joins
{
    public class MediaGenre
    {
        public Guid MediaId { get; set; }
        public Media Media { get; set; }
        public Guid GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
