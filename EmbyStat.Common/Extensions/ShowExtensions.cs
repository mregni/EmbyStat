using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Shows;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Common.Extensions
{
    public static class ShowExtensions
    {
        /// <summary>
        /// Checks if any episodes are changed since the last sync for a show
        /// </summary>
        /// <param name="show">New show data from the external server</param>
        /// <param name="oldShow">Internal show data on which a comparison is required</param>
        /// <returns>True or false if episodes have changed</returns>
        public static bool HasShowChangedEpisodes(this SqlShow show, SqlShow oldShow)
        {
            if (oldShow == null)
            {
                return true;
            }

            var oldShowEpisodes = oldShow.Seasons.SelectMany(x => x.Episodes.Select(y => y.Id));
            var newShowEpisodes = show.Seasons.SelectMany(x => x.Episodes.Select(y => y.Id));

            return !oldShowEpisodes.AreListEqual(newShowEpisodes);
        }

        /// <summary>
        /// Calculates the total Mb size of a show by making the total SUM of the first MediaSource for each Episode
        /// </summary>
        /// <param name="show">Show for which the total size needs to be calculated</param>
        /// <returns>Total space used for the show in Mb </returns>
        public static double GetShowSize(this SqlShow show)
        {
            return show.Seasons.SelectMany(x => x.Episodes)
                .Where(x => x.LocationType == LocationType.Disk)
                .Sum(x => x.MediaSources.FirstOrDefault()?.SizeInMb ?? 0);
        }

        /// <summary>
        /// Calculates the total runtime length of a show by making the total SUM of all the RunTimeTicks for each Episode
        /// </summary>
        /// <param name="show">Show for which the total runtime needs to be calculated</param>
        /// <returns>Total runtime ticks for the show</returns>
        public static long GetShowRunTimeTicks(this SqlShow show)
        {
            return show.Seasons.SelectMany(x => x.Episodes)
                .Where(x => x.LocationType == LocationType.Disk)
                .Sum(x => x.RunTimeTicks ?? 0);
        }

        /// <summary>
        /// Generates a COUNT(*) query for the show table
        /// </summary>
        /// <param name="list">Db set on which to create the query on</param>
        /// <param name="filters">Filters that need to be applied in the query</param>
        /// <param name="libraryIds">libraries for which the query should filter</param>
        /// <returns>Sqlite query that can query the count of shows</returns>
        public static string GenerateCountQuery(this DbSet<SqlShow> list, Filter[] filters, IReadOnlyList<string> libraryIds)
        {
            var query = $@"
SELECT COUNT() AS Count
FROM {Constants.Tables.Shows} as s
WHERE 1=1 {libraryIds.AddLibraryIdFilterAsAnd("s")}
";
            query = filters.Aggregate(query, (current, filter) => current + AddShowFilter(filter));

            return query;
        }

        private static string AddShowFilter(Filter filter)
        {
            return $"AND {GenerateFilterString(filter)}\n";
        }

        private static string GenerateFilterString(Filter filter)
        {
            switch (filter.Field)
            {
                default:
                    return filter.Operation switch
                    {
                        "==" => $"m.{filter.Field} = '{filter.Value}'",
                        "!=" => $"m.{filter.Field} != '{filter.Value}'",
                        "contains" => $"m.{filter.Field} LIKE '%{filter.Value}%'",
                        "!contains" => $"m.{filter.Field} NOT LIKE '%{filter.Value}%'",
                        "startsWith" => $"m.{filter.Field} LIKE '{filter.Value}%')",
                        "endsWith" => $"m.{filter.Field} LIKE '%{filter.Value}')",
                        "null" => $"m.{filter.Field} IS NULL OR m.{filter.Field} = ''",
                        _ => string.Empty
                    };
            }
        }

        public static string GenerateFullShowQuery(this DbSet<SqlShow> shows, IEnumerable<string> libraryIds, bool includeEpisodes)
        {
            var queryBuilder = new StringBuilder("SELECT s.*");

            queryBuilder.Append(AddString(includeEpisodes, ", se.*, e.*"));
            queryBuilder.Append($" FROM {Constants.Tables.Shows} AS s");

            var seasonTable = $" LEFT JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)";
            var episodeTable = $" LEFT JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)";
            queryBuilder.Append(AddString(includeEpisodes, $"{seasonTable}{episodeTable}"));

            queryBuilder.Append($" WHERE 1=1 {libraryIds.AddLibraryIdFilterAsAnd("s")}");
            return queryBuilder.ToString();
        }

        public static string GenerateFullShowQuery(this DbSet<SqlShow> shows, bool includeEpisodes)
        {
            var queryBuilder = new StringBuilder("SELECT s.*");

            queryBuilder.Append(AddString(includeEpisodes, ", se.*, e.*"));
            queryBuilder.Append($" FROM {Constants.Tables.Shows} AS s");

            var seasonTable = $" LEFT JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)";
            var episodeTable = $" LEFT JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)";
            queryBuilder.Append(AddString(includeEpisodes, $"{seasonTable}{episodeTable}"));

            queryBuilder.Append($" WHERE s.Id = @Id");
            return queryBuilder.ToString();
        }

        private static string AddString(bool includeEpisodes, string value)
        {
            return includeEpisodes ? value : string.Empty;
        }
    }
}
