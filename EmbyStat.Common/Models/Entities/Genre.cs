using System.Collections.Generic;
using EmbyStat.Common.Models.Entities.Movies;

namespace EmbyStat.Common.Models.Entities
{
    public class Genre
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<Movie> Movies { get; set; }
        public ICollection<Shows.Show> Shows { get; set; }
    }
}
