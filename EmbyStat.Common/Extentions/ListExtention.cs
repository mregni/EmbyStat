using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbyStat.Common.Extentions
{
    public static class ListExtention
    {
        public static bool AreListEqual<T>(this List<T> listA, List<T> listB)
        {
            return listA.All(listB.Contains) && listA.Count == listB.Count;
        }
    }
}
