using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbyStat.Common.Extentions
{
    public static class ListExtention
    {
        public static bool AreListEqual<T>(this IEnumerable<T> listA, IEnumerable<T> listB)
        {
            var a = listA.ToList();
            var b = listB.ToList();
            return a.All(b.Contains) && a.Count == b.Count;
        }
    }
}
