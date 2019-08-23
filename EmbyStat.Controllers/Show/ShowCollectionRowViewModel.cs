using System;
using System.Collections.Generic;

namespace EmbyStat.Controllers.Show
{
    public class ShowCollectionRowViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Seasons { get; set; }
        public int Episodes { get; set; }
        public int Specials { get; set; }
        public IEnumerable<EpisodeViewModel> MissingEpisodes { get; set; }
        public DateTimeOffset? PremiereDate { get; set; }
        public bool Status { get; set; }
        public string SortName { get; set; }
    }
}
