using System.Collections.Generic;
using System.Linq;

namespace EmbyStat.Common.Extensions;

public static class ListExtensions
{
    public static bool AreListEqual<T>(this IEnumerable<T> listA, IEnumerable<T> listB)
    {
        var a = listA.ToList();
        var b = listB.ToList();
        return a.All(b.Contains) && a.Count == b.Count;
    }

    public static bool AnyNotNull<TSource>(this IEnumerable<TSource> source)
    {
        return source != null && source.Any();
    }

    public static void AddIfNotNull<T>(this ICollection<T> items, T item)
    {
        if (item != null)
        {
            items.Add(item);
        }
    }

    public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            list.Add(item);
        }
    }
}