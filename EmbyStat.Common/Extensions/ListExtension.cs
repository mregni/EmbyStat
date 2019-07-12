using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;

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

        public static IEnumerable<Show> GetNeverSyncedShows(this IEnumerable<Show> shows)
        {
            return shows.Where(x => !x.TvdbSynced || x.TvdbFailed).ToList();
        }

        public static IEnumerable<Show> GetShowsWithChangedEpisodes(this IEnumerable<Show> shows, IReadOnlyList<Show> oldShows)
        {
            foreach (var show in shows)
            {
                var oldShow = oldShows.SingleOrDefault(x => x.Id == show.Id);
                if (oldShow == null)
                {
                    continue;
                }

                if (!oldShow.Episodes.Select(x => x.Id).AreListEqual(show.Episodes.Select(x => x.Id)))
                {
                    yield return show;
                }
            }
        }
    }
}
