using System.Collections.Generic;
using System.Linq;

namespace EmbyStat.Common
{
    public class ChartGrouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
    {
        public TKey Key
        {
            get;
            set;
        }
    }
}
