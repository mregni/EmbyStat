using System.Collections.Generic;

namespace EmbyStat.Controllers.Movie
{
    public class SuspiciousTablesViewModel
    {
        public List<ShortMovieViewModel> Shorts { get; set; }
        public List<SuspiciousMovieViewModel> NoImdb { get; set; }
        public List<SuspiciousMovieViewModel> NoPrimary { get; set; }
    }
}
