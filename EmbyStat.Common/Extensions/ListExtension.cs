using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using EmbyStat.Common.Enums;
using LiteDB;

namespace EmbyStat.Common.Extensions
{
    public static class ListExtension
    {
        public static bool AreListEqual<T>(this IEnumerable<T> listA, IEnumerable<T> listB)
        {
            var a = listA.ToList();
            var b = listB.ToList();
            return a.All(b.Contains) && a.Count == b.Count;
        }

        public static IEnumerable<IEnumerable<T>> PowerSets<T>(this IList<T> set)
        {
            var totalSets = BigInteger.Pow(2, set.Count);
            for (BigInteger i = 0; i < totalSets; i++)
            {
                yield return set.SubSet(i);
            }
        }

        private static IEnumerable<T> SubSet<T>(this IList<T> set, BigInteger n)
        {
            for (var i = 0; i < set.Count && n > 0; i++)
            {
                if ((n & 1) == 1)
                {
                    yield return set[i];
                }

                n = n >> 1;
            }
        }

        public static bool AnyNotNull<TSource>(this IEnumerable<TSource> source)
        {
            return source != null && source.Any();
        }

        public static BsonArray ConvertToBsonArray(this IEnumerable<string> items)
        {
            var bArray = new BsonArray();
            foreach (var item in items)
            {
                bArray.Add(item);
            }

            return bArray;
        }

        public static BsonArray ConvertToBsonArray(this IEnumerable<LibraryType> items)
        {
            var bArray = new BsonArray();
            foreach (var item in items)
            {
                bArray.Add(item.ToString());
            }

            return bArray;
        }

        public static BsonArray ConvertToBsonArray(this IEnumerable<int> items)
        {
            var bArray = new BsonArray();
            foreach (var item in items)
            {
                bArray.Add(item);
            }

            return bArray;
        }

        public static string AddLibraryIdFilter(this IEnumerable<string> libraryIds, string collectionAbr)
        {
            return libraryIds.Any() ? $"{collectionAbr}.CollectionId IN @Ids" : string.Empty;
        }

        public static string AddLibraryIdFilterAsAnd(this IEnumerable<string> libraryIds, string collectionAbr)
        {
            return libraryIds.Any() ? $"AND {collectionAbr}.CollectionId IN @Ids" : string.Empty;
        }

        public static void AddIfNotNull<T>(this ICollection<T> items, T item)
        {
            if (item != null)
            {
                items.Add(item);
            }
        }
    }
}
