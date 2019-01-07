using System;

namespace EmbyStat.Services.Models.Movie
{
    public class SuspiciousMovie
    {
        public Guid MediaId { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
    }
}
