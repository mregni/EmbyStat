using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models
{
    public class Movie : Video
    {
        public string HomePageUrl { get; set; }
        public string OfficialRating { get; set; }
        public string OriginalTitle { get; set; }
    }
}
