using System;
using System.Collections.Generic;
using EmbyStat.Common.SqLite.Helpers;

namespace EmbyStat.Common.SqLite
{
    public class SqlPerson
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? MovieCount { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? ShowCount { get; set; }
        public string Primary { get; set; }
        public ICollection<SqlMediaPerson> MediaPeople { get; set; }
    }
}
