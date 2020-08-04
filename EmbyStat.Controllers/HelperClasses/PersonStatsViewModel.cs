using System.Collections.Generic;

namespace EmbyStat.Controllers.HelperClasses
{
    public class PersonStatsViewModel
    {
        public List<CardViewModel<string>> Cards { get; set; }
        public List<TopCardViewModel> GlobalCards { get; set; }
        public List<TopCardViewModel> MostFeaturedActorsPerGenreCards { get; set; }
    }
}
