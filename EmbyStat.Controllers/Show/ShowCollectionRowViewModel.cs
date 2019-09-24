using System;
using System.Collections.Generic;

namespace EmbyStat.Controllers.Show
{
    public class ShowCollectionRowViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TVDB { get; set; }
        public string IMDB { get; set; }
        public double Size { get; set; }
        public string Banner { get; set; }
        public int Seasons { get; set; }
        public int Episodes { get; set; }
        public int Specials { get; set; }
        public int MissingEpisodeCount { get; set; }
        public IEnumerable<VirtualSeasonViewModel> MissingEpisodes { get; set; }
        public DateTimeOffset? PremiereDate { get; set; }
        public bool Status { get; set; }
        public string SortName { get; set; }
    }
}
