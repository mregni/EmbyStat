using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Movie
{
    public class ShortMovie
    {
        public int Number { get; set; }
        public Guid MediaId { get; set; }
        public string Title { get; set; }
        public double Duration { get; set; }
    }
}
