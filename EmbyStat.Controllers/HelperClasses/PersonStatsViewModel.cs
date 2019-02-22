using System.Collections.Generic;

namespace EmbyStat.Controllers.HelperClasses
{
    public class PersonStatsViewModel
    {
        public CardViewModel<int> TotalActorCount { get; set; }
        public CardViewModel<int> TotalDirectorCount { get; set; }
        public CardViewModel<int> TotalWriterCount { get; set; }
        public PersonPosterViewModel MostFeaturedActor { get; set; }
        public PersonPosterViewModel MostFeaturedDirector { get; set; }
        public PersonPosterViewModel MostFeaturedWriter { get; set; }
        public List<PersonPosterViewModel> MostFeaturedActorsPerGenre { get; set; }
    }
}
