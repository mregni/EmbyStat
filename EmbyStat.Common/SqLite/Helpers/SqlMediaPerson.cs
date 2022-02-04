using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Shows;

namespace EmbyStat.Common.SqLite.Helpers
{
    public class SqlMediaPerson : IEqualityComparer<SqlMediaPerson>
    {
        public PersonType Type { get; set; }
        public int Id { get; set; }
        public SqlMovie Movie { get; set; }
        public string MovieId { get; set; }
        public SqlShow Show { get; set; }
        public string ShowId { get; set; }
        public SqlEpisode Episode { get; set; }
        public string EpisodeId { get; set; }
        public SqlPerson Person { get; set; }
        public string PersonId { get; set; }

        public bool Equals(SqlMediaPerson x, SqlMediaPerson y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.PersonId == y.PersonId;
        }

        public int GetHashCode(SqlMediaPerson obj)
        {
            return (obj.PersonId != null ? obj.PersonId.GetHashCode() : 0);
        }
    }
}
