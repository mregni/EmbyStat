using System.Collections.Generic;

namespace EmbyStat.Services.Models.Stat
{
    public class PersonStats
    {
        public List<Card<string>> Cards { get; set; }
        public List<PersonPoster> Posters { get; set; }
        public List<PersonPoster> MostFeaturedActorsPerGenre { get; set; }
    }
}
