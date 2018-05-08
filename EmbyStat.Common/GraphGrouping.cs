using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
