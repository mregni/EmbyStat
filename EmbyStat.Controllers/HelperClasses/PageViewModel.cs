using System.Collections.Generic;

namespace EmbyStat.Controllers.HelperClasses
{
    public class PageViewModel<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
