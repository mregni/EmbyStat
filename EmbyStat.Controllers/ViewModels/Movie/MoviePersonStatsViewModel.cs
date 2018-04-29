using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Controllers.ViewModels.Stat;

namespace EmbyStat.Controllers.ViewModels.Movie
{
    public class MoviePersonStatsViewModel
    {
        public CardViewModel TotalActorCount { get; set; }
        public CardViewModel TotalDirectorCount { get; set; }
        public CardViewModel TotalWriterCount { get; set; }
        public PersonPosterViewModel MostFeaturedActor { get; set; }
        public PersonPosterViewModel MostFeaturedDirector { get; set; }
        public PersonPosterViewModel MostFeaturedWriter { get; set; }
        public List<PersonPosterViewModel> MostFeaturedActorsPerGenre { get; set; }
    }
}
