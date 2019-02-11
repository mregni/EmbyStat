using System.Collections.Generic;

namespace EmbyStat.Controllers.HelperClasses
{
    public class PersonStatsViewModel
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
