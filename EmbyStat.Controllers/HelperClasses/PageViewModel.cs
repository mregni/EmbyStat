using System.Collections.Generic;

namespace EmbyStat.Controllers.HelperClasses
{
    public class PageViewModel<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
