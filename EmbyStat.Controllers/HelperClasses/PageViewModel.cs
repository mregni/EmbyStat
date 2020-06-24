using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.HelperClasses
{
    public class PageViewModel<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
