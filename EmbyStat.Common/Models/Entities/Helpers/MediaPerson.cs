using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Movies;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class MediaPerson : IEqualityComparer<MediaPerson>
    {
        public PersonType Type { get; set; }
        public int Id { get; set; }
        public Movie Movie { get; set; }
        public string MovieId { get; set; }
        public Shows.Show Show { get; set; }
        public string ShowId { get; set; }
        public Person Person { get; set; }
        public string PersonId { get; set; }

        public bool Equals(MediaPerson x, MediaPerson y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.PersonId == y.PersonId;
        }

        public int GetHashCode(MediaPerson obj)
        {
            return (obj.PersonId != null ? obj.PersonId.GetHashCode() : 0);
        }
    }
}
