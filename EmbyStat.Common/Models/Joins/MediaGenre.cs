using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models.Joins
{
    public class MediaGenre
    {
        public string MediaId { get; set; }
        public Media Media { get; set; }
        public string GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
