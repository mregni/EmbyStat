using System.Collections.Generic;
using EmbyStat.Common.Enums;

namespace EmbyStat.Common.SqLite.Movies
{
    public class SqlMoviePerson : IEqualityComparer<SqlMoviePerson>
    {
        public PersonType Type { get; set; }
        public SqlMovie Movie { get; set; }
        public string MovieId { get; set; }
        public SqlPerson Person { get; set; }
        public string PersonId { get; set; }

        public bool Equals(SqlMoviePerson x, SqlMoviePerson y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.PersonId == y.PersonId;
        }

        public int GetHashCode(SqlMoviePerson obj)
        {
            return (obj.PersonId != null ? obj.PersonId.GetHashCode() : 0);
        }
    }
}
