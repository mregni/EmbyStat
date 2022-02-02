using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Streams;

namespace EmbyStat.Common.SqLite.Shows
{
    public class SqlEpisode : SqlVideo
    {
        public ICollection<SqlShowSqlPerson> ShowPeople { get; set; }
        public float? DvdEpisodeNumber { get; set; }
        public int? DvdSeasonNumber { get; set; }
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
        public int? SeasonIndexNumber { get; set; }
        public LocationType LocationType { get; set; }
        public SqlSeason Season { get; set; }
        public string SeasonId { get; set; }

    }
}
