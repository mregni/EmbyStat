using System.Collections.Generic;

namespace EmbyStat.Services.Models.Charts
{
    public class Chart
    {
        public string Title { get; set; }
        public IEnumerable<string> Labels { get; set; }
        public List<IEnumerable<int>> DataSets { get; set; }
    }
}
