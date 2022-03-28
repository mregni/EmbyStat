using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Common.Extensions
{
    public static class SqlMediaExtensions
    {
        /// <summary>
        /// Generates a query <see cref="string"/> that can query a top list ordered by premiere date
        /// </summary>
        /// <param name="list"><see cref="DbSet{T}"/> where T is of type <see cref="Extra"/> on which the query is generated</param>
        /// <param name="count">Amount of rows returned</param>
        /// <param name="orderDirection">asc or desc depending on the sorting needed</param>
        /// <param name="table">Table name on which to perform the query</param>
        /// <returns>Sqlite query <see cref="string"/> that can query shows ordered by premiereDate</returns>
        public static string GenerateGetPremieredListQuery<T>(this DbSet<T> list, int count, string orderDirection, string table) where T : Extra
        {
            return $@"SELECT s.*
FROM {table} AS s
WHERE s.PremiereDate IS NOT NULL 
ORDER BY PremiereDate {orderDirection}
LIMIT {count}";
        }

        /// <summary>
        /// Returns list of <see cref="Media"/> ordered by creation date
        /// </summary>
        /// <param name="list"><see cref="DbSet{T}"/> where T is of type <see cref="Extra"/> on which the query is executed</param>
        /// <param name="count">Amount of rows returned</param>
        /// <typeparam name="T">T should be of type <see cref="Media"/></typeparam>
        /// <returns>Returns <see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<T> GetLatestAddedMedia<T>(this DbSet<T> list, int count) where T : Media
        {
            return list
                .Where(x => x.DateCreated.HasValue)
                .OrderByDescending(x => x.DateCreated)
                .Take(count);
        }

        /// <summary>
        /// Generates a query <see cref="string"/> that can query a top list ordered by community rating
        /// </summary>
        /// <param name="list"><see cref="DbSet{T}"/> where T is of type <see cref="Extra"/> on which the query is generated</param>
        /// <param name="count">Amount of rows returned</param>
        /// <param name="orderDirection">asc or desc depending on the sorting needed</param>
        /// <param name="table">Table name on which to perform the query</param>
        /// <returns>Sqlite query <see cref="string"/> that can query shows ordered by communityRating</returns>
        public static string GenerateGetCommunityRatingListQuery<T>(this DbSet<T> list, int count, string orderDirection, string table) where T : Extra
        {
            return $@"SELECT s.*
FROM {table} AS s
WHERE s.CommunityRating IS NOT NULL
ORDER BY CommunityRating {orderDirection}
LIMIT {count}";
        }

        /// <summary>
        /// Adds genres to a <see cref="IEnumerable{T}"/> of type <see cref="ILinkedMedia"/> from <see cref="Genre"/>
        /// </summary>
        /// <param name="items"><see cref="IEnumerable{T}"/> where T is <see cref="ILinkedMedia"/></param>
        /// <param name="genres">List of <see cref="Genre"/> that needs to be processed</param>
        /// <typeparam name="T"><see cref="ILinkedMedia"/></typeparam>
        public static void AddGenres<T>(this IEnumerable<T> items, IEnumerable<Genre> genres) where T : ILinkedMedia
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