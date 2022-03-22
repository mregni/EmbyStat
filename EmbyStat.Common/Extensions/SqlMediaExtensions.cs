using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Common.Extensions
{
    public static class SqlMediaExtensions
    {
        /// <summary>
        /// Generates a query <see cref="string"/> that can query a top list ordered by premiere date
        /// </summary>
        /// <param name="list"><see cref="DbSet{T}"/> where T is of type <see cref="SqlExtra"/> on which the query is generated</param>
        /// <param name="libraryIds">Libraries for which the query should filter</param>
        /// <param name="count">Amount of rows returned</param>
        /// <param name="orderDirection"></param>
        /// <param name="sortOrder">asc or desc depending on the sorting needed</param>
        /// <returns>Sqlite query <see cref="string"/> that can query shows ordered by premiereDate</returns>
        public static string GenerateGetPremieredListQuery<T>(this DbSet<T> list, IReadOnlyList<string> libraryIds,
            int count, string orderDirection) where T : SqlExtra
        {
            return $@"SELECT s.*
FROM {Constants.Tables.Shows} AS s
WHERE s.PremiereDate IS NOT NULL {libraryIds.AddLibraryIdFilterAsAnd("s")}
ORDER BY PremiereDate {orderDirection}
LIMIT {count}";
        }

        /// <summary>
        /// Returns list of <see cref="SqlMedia"/> ordered by creation date
        /// </summary>
        /// <param name="list"><see cref="DbSet{T}"/> where T is of type <see cref="SqlExtra"/> on which the query is executed</param>
        /// <param name="libraryIds">Libraries for which the query should filter</param>
        /// <param name="count">Amount of rows returned</param>
        /// <typeparam name="T">T should be of type <see cref="SqlMedia"/></typeparam>
        /// <returns>Returns <see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<T> GetLatestAddedMedia<T>(this DbSet<T> list, IReadOnlyList<string> libraryIds,
            int count)
            where T : SqlMedia
        {
            return list
                .FilterOnLibrary(libraryIds)
                .Where(x => x.DateCreated.HasValue)
                .OrderByDescending(x => x.DateCreated)
                .Take(count);
        }

        /// <summary>
        /// Generates a query <see cref="string"/> that can query a top list ordered by community rating
        /// </summary>
        /// <param name="list"><see cref="DbSet{T}"/> where T is of type <see cref="SqlExtra"/> on which the query is generated</param>
        /// <param name="libraryIds">Libraries for which the query should filter</param>
        /// <param name="count">Amount of rows returned</param>
        /// <param name="orderDirection"></param>
        /// <param name="sortOrder">asc or desc depending on the sorting needed</param>
        /// <returns>Sqlite query <see cref="string"/> that can query shows ordered by communityRating</returns>
        public static string GenerateGetCommunityRatingListQuery<T>(this DbSet<T> list,
            IReadOnlyList<string> libraryIds,
            int count, string orderDirection) where T : SqlExtra
        {
            return $@"SELECT s.*
FROM {Constants.Tables.Shows} AS s
WHERE s.CommunityRating IS NOT NULL {libraryIds.AddLibraryIdFilterAsAnd("s")}
ORDER BY CommunityRating {orderDirection}
LIMIT {count}";
        }

        /// <summary>
        /// Adds genres to a <see cref="IEnumerable{T}"/> of type <see cref="ISqlLinked"/> from <see cref="SqlGenre"/>
        /// </summary>
        /// <param name="items"><see cref="IEnumerable{T}"/> where T is <see cref="ISqlLinked"/></param>
        /// <param name="genres">List of <see cref="SqlGenre"/> that needs to be processed</param>
        /// <typeparam name="T"><see cref="ISqlLinked"/></typeparam>
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