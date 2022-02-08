using System.Collections.Generic;

namespace EmbyStat.Common.SqLite.Helpers
{
    public interface ISqlLinked
    {
        ICollection<SqlMediaPerson> People { get; set; }
        ICollection<SqlGenre> Genres { get; set; }
    }
}
