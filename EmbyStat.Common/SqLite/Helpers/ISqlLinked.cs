using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.SqLite.Helpers
{
    public interface ISqlLinked
    {
        ICollection<SqlMediaPerson> People { get; set; }
        ICollection<SqlGenre> Genres { get; set; }
        Library Library { get; set; }
    }
}
