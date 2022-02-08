using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Streams;

namespace EmbyStat.Common.SqLite.Movies
{
    public class SqlMovie : SqlVideo, ISqlLinked
    {
        public string OriginalTitle { get; set; }

        public override bool Equals(object? other)
        {
            if (other is SqlMovie media)
            {
                return Id == media.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        #region Inherited props
        public ICollection<SqlMediaPerson> People { get; set; }
        public ICollection<SqlGenre> Genres { get; set; }

        #endregion
    }
}
