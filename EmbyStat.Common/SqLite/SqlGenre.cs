using System.Collections.Generic;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Shows;

namespace EmbyStat.Common.SqLite
{
    public class SqlGenre
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<SqlMovie> Movies { get; set; }
        public ICollection<SqlShow> Shows { get; set; }
    }
}
