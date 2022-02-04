using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.SqLite.Helpers;

namespace EmbyStat.Common.SqLite.Shows
{
    public class SqlShow : SqlExtra
    {
        public long? CumulativeRunTimeTicks { get; set; }
        public string Status { get; set; }
        public bool ExternalSyncFailed { get; set; }
        public double SizeInMb { get; set; }
        public ICollection<SqlSeason> Seasons { get; set; }
    }
}
