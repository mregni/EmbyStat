using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.HelperClasses
{
    public class ListContainer<T>
    {
        public int TotalCount { get; set; }
        public List<T> Data { get; set; }
    }
}
