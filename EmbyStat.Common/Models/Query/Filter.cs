using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Models.Query
{
    public class Filter
    {
        public string Value { get; set; }
        public string Operation { get; set; }
        public string Field { get; set; }
    }
}
