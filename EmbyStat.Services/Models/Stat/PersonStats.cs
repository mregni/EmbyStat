using System.Collections.Generic;
using EmbyStat.Services.Models.Cards;

namespace EmbyStat.Services.Models.Stat
{
    public class PersonStats
    {
        public List<Card<string>> Cards { get; set; }
        public List<TopCard> GlobalCards { get; set; }
        public List<TopCard> MostFeaturedActorsPerGenreCards { get; set; }

        public PersonStats()
        {
            Cards = new List<Card<string>>();
            GlobalCards = new List<TopCard>();
            MostFeaturedActorsPerGenreCards = new List<TopCard>();
        }
    }
}
