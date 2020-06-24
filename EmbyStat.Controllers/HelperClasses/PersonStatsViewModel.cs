using System.Collections.Generic;

namespace EmbyStat.Controllers.HelperClasses
{
    public class PersonStatsViewModel
    {
        public List<CardViewModel<string>> Cards { get; set; }
        public List<PersonPosterViewModel> Posters { get; set; }
        public List<PersonPosterViewModel> MostFeaturedActorsPerGenre { get; set; }
    }
}
