using System.Collections.Generic;
using System.Linq;

namespace EmbyStat.Common
{
    public class GraphGrouping<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
    {
        public TKey Key
        {
            get;
            set;
        }
    }
}
