using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbyStat.Common.SqLite.Helpers
{
    public abstract class SqlExtra : SqlMedia
    {
        public float? CommunityRating { get; set; }
        public string IMDB { get; set; }
        public int? TMDB { get; set; }
        public string TVDB { get; set; }
        public long? RunTimeTicks { get; set; }
        public string OfficialRating { get; set; }
        public ICollection<SqlGenre> Genres { get; set; }
        public ICollection<SqlMediaPerson> People { get; set; }
    }
}
