using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Services.Models.Show
{
    public class ShowCollectionRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Tvdb { get; set; }
        public string Imdb { get; set; }
        public double Size { get; set; }
        public string Banner { get; set; }
        public int Seasons { get; set; }
        public int Episodes { get; set; }
        public int Specials { get; set; }
        public int MissingEpisodeCount { get; set; }
        public IEnumerable<VirtualSeason> MissingEpisodes { get; set; }
        public DateTimeOffset? PremiereDate { get; set; }
        public bool Status { get; set; }
        public string SortName { get; set; }
    }
}
