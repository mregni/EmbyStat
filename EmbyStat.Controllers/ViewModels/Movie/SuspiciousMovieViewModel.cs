using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.ViewModels.Movie
{
    public class SuspiciousMovieViewModel
    {
        public Guid MediaId { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
    }
}
