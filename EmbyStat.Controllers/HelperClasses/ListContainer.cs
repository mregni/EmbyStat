using System.Collections.Generic;

namespace EmbyStat.Controllers.HelperClasses
{
    public class ListContainer<T>
    {
        public int TotalCount { get; set; }
        public List<T> Data { get; set; }
    }
}
