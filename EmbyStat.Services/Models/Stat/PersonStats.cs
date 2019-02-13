using System.Collections.Generic;

namespace EmbyStat.Services.Models.Stat
{
    public class PersonStats
    {
        public Card<int> TotalActorCount { get; set; }
        public Card<int> TotalDirectorCount { get; set; }
        public Card<int> TotalWriterCount { get; set; }
        public PersonPoster MostFeaturedActor { get; set; }
        public PersonPoster MostFeaturedDirector { get; set; }
        public PersonPoster MostFeaturedWriter { get; set; }
        public List<PersonPoster> MostFeaturedActorsPerGenre { get; set; }
    }
}
