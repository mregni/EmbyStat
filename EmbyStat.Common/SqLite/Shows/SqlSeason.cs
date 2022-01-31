using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Helpers;

namespace EmbyStat.Common.SqLite.Shows
{
    public class SqlSeason : SqlMedia
    {
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
        public LocationType LocationType { get; set; }
        public ICollection<SqlEpisode> Episodes { get; set; }
        public SqlShow Show { get; set; }
        public string ShowId { get; set; }
    }
}
