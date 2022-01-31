using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Movies;

namespace EmbyStat.Common.SqLite.Shows
{
    public class SqlShowSqlPerson
    {
        public PersonType Type { get; set; }
        public SqlShow Show { get; set; }
        public string ShowId { get; set; }
        public SqlPerson Person { get; set; }
        public string PersonId { get; set; }
    }
}
