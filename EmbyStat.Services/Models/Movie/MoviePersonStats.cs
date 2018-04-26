using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Movie
{
    public class MoviePersonStats
    {
        public Card TotalActorCount { get; set; }
        public Card TotalDirectorCount { get; set; }
        public Card TotalWriterCount { get; set; }
        public PersonPoster MostFeaturedActor { get; set; }
        public PersonPoster MostFeaturedDirector { get; set; }
        public PersonPoster MostFeaturedWriter { get; set; }
    }
}
