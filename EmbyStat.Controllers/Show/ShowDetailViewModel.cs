﻿using System;
using System.Collections.Generic;

namespace EmbyStat.Controllers.Show
{
    public class ShowDetailViewModel
    {
        public string Id { get; set; }
        public long? CumulativeRunTimeTicks { get; set; }
        public string Status { get; set; }
        public int SeasonCount { get; set; }
        public IEnumerable<VirtualSeasonViewModel> MissingSeasons { get; set; }
        public int EpisodeCount { get; set; }
        public int SpecialEpisodeCount { get; set; }
        public string[] Genres { get; set; }
        public string Imdb { get; set; }
        public int? Tmdb { get; set; }
        public string Tvdb { get; set; }
        public float? CommunityRating { get; set; }
        public long? RunTimeTicks { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Primary { get; set; }
        public string Thumb { get; set; }
        public string Name { get; set; }
        public int? ProductionYear { get; set; }
        public DateTime? PremiereDate { get; set; }
        public string Path { get; set; }
        public double SizeInMb { get; set; }

    }
}
