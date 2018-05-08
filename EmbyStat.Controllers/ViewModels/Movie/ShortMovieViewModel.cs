using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.ViewModels.Movie
{
    public class ShortMovieViewModel
    {
        public int Number { get; set; }
        public string MediaId { get; set; }
        public string Title { get; set; }
        public double Duration { get; set; }
    }
}
