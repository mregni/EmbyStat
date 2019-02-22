using System.Collections.Generic;

namespace EmbyStat.Controllers.HelperClasses
{
    public class GraphViewModel<T> where T : class
    {
        public string Title { get; set; }
        public List<T> Data { get; set; }
    }
}
