using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Common.Extensions
{
    public static class SqlMediaExtensions 
    {
        public static IEnumerable<T> GetNewestPremieredMedia<T>(this DbSet<T> list,
            IReadOnlyList<string> libraryIds, int count) where T : SqlMedia
        {
            return list
                .FilterOnLibrary(libraryIds)
                .Where(x => x.PremiereDate.HasValue)
                .OrderByDescending(x => x.PremiereDate)
                .Take(count);
        }

        public static IEnumerable<T> GetOldestPremieredMedia<T>(this DbSet<T> list, 
            IReadOnlyList<string> libraryIds, int count) where T : SqlMedia
        {
            return list
                .FilterOnLibrary(libraryIds)
                .Where(x => x.PremiereDate.HasValue)
                .OrderBy(x => x.PremiereDate)
                .Take(count);
        }

        public static IEnumerable<T> GetLatestAddedMedia<T>(this DbSet<T> list, IReadOnlyList<string> libraryIds, int count)
            where T : SqlMedia
        {
            return list
                .FilterOnLibrary(libraryIds)
                .Where(x => x.DateCreated.HasValue)
                .OrderByDescending(x => x.DateCreated)
                .Take(count);
        }

        public static IEnumerable<T> GetHighestRatedMedia<T>(this DbSet<T> list, IReadOnlyList<string> libraryIds,
            int count) where T : SqlExtra
        {
            return list.FilterOnLibrary(libraryIds)
                .Where(x => x.CommunityRating.HasValue)
                .OrderByDescending(x => x.CommunityRating)
                .Take(count);
        }

        public static IEnumerable<T> GetLowestRatedMedia<T>(this DbSet<T> list, IReadOnlyList<string> libraryIds, int count) where T : SqlExtra
        {
            return list
                .FilterOnLibrary(libraryIds)
                .Where(x => x.CommunityRating.HasValue)
                .OrderBy(x => x.CommunityRating)
                .Take(count);
        }

        public static void AddGenres<T>(this IEnumerable<T> items, IEnumerable<SqlGenre> genres) where T : ISqlLinked
        {
            var genreList = genres.ToList();
            foreach (var item in items)
            {
                if (item.Genres == null || !item.Genres.Any())
                {
                    continue;
                }

                foreach (var dtoGenre in item.Genres)
                {
                    var localGenre = genreList.FirstOrDefault(x => x.Name == dtoGenre.Name);
                    if (localGenre != null)
                    {
                        dtoGenre.Id = localGenre.Id;
                    }
                }
            }
        }
    }
}
