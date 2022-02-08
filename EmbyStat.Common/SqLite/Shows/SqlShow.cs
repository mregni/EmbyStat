using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.SqLite.Helpers;

namespace EmbyStat.Common.SqLite.Shows
{
    public class SqlShow : SqlExtra, ISqlLinked
    {
        public long? CumulativeRunTimeTicks { get; set; }
        public string Status { get; set; }
        public bool ExternalSynced { get; set; }
        public double SizeInMb { get; set; }
        public ICollection<SqlSeason> Seasons { get; set; }

        #region Inherited props
        public ICollection<SqlMediaPerson> People { get; set; }
        public ICollection<SqlGenre> Genres { get; set; }
        #endregion
    }
}
