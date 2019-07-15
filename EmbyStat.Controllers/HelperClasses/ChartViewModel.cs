using System.Collections.Generic;

namespace EmbyStat.Controllers.HelperClasses
{
    public class ChartViewModel
    {
        public string Title { get; set; }
        public IEnumerable<string> Labels { get; set; }
        public List<IEnumerable<int>> DataSets { get; set; }
    }
}
