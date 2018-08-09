﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.ViewModels.Movie
{
    public class SuspiciousTablesViewModel
    {
        public List<MovieDuplicateViewModel> Duplicates { get; set; }
        public List<ShortMovieViewModel> Shorts { get; set; }
        public List<SuspiciousMovieViewModel> NoImdb { get; set; }
        public List<SuspiciousMovieViewModel> NoPrimary { get; set; }
    }
}
