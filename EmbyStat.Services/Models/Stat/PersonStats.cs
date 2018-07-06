using System.Collections.Generic;

namespace EmbyStat.Services.Models.Stat
{
    public class PersonStats
    {
        public Card TotalActorCount { get; set; }
        public Card TotalDirectorCount { get; set; }
        public Card TotalWriterCount { get; set; }
        public PersonPoster MostFeaturedActor { get; set; }
        public PersonPoster MostFeaturedDirector { get; set; }
        public PersonPoster MostFeaturedWriter { get; set; }
        public List<PersonPoster> MostFeaturedActorsPerGenre { get; set; }
    }
}
