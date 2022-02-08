using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbyStat.Common.SqLite.Helpers
{
    public interface ISqlLinked
    {
        ICollection<SqlMediaPerson> People { get; set; }
        ICollection<SqlGenre> Genres { get; set; }
    }
}
