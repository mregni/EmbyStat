using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbyStat.Common.SqLite
{
    public class SqlGenre
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<SqlMovie> Movies { get; set; }
    }
}
