using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbyStat.Common.SqLite
{
    public class SqlMovieGenre
    {
        public string Id { get; set; }
        public SqlMovie Movie { get; set; }
        public string MovieId { get; set; }
        public SqlGenre Genre { get; set; }
        public string GenreId { get; set; }
    }
}
