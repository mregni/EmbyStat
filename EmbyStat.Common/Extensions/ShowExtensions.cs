using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <param name="libraryIds">Libraries for which the query should filter</param>
        /// <returns>Sqlite query that can query the count of shows</returns>
        public static string GenerateCountQuery(this DbSet<SqlShow> list, Filter[] filters,
            IReadOnlyList<string> libraryIds)
        {
            var query = $@"
SELECT COUNT() AS Count
FROM {Constants.Tables.Shows} as s
WHERE 1=1 {libraryIds.AddLibraryIdFilterAsAnd("s")}
";
            query = filters.Aggregate(query, (current, filter) => current + AddShowFilters(filter));

            return query;
        }

        /// <summary>
        /// Generates a full LEFT JOIN sql query for shows, seasons and episodes. Genres or streams are not included in the query
        /// </summary>
        /// <param name="shows">Db set on which to create the query on</param>
        /// <param name="includeEpisodes">Includes seasons and episodes in the query if set to True</param>
        /// <param name="libraryIds">Libraries for which the query should filter</param>
        /// <returns>Sqlite query that can query shows</returns>
        public static string GenerateFullShowQuery(this DbSet<SqlShow> shows, IEnumerable<string> libraryIds)
        {
            return $@"
SELECT s.*, g.*, se.*, e.*
FROM {Constants.Tables.Shows} as s
LEFT JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)
LEFT JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)
WHERE 1=1 {libraryIds.AddLibraryIdFilterAsAnd("s")}";
        }

        public static string GenerateFullShowWithGenresQuery(this DbSet<SqlShow> shows, IEnumerable<string> libraryIds)
        {
            return $@"
SELECT s.*, g.*, se.*, e.*
FROM {Constants.Tables.Shows} as s
LEFT JOIN {Constants.Tables.GenreShow} AS gs ON (gs.ShowsId = s.Id)
LEFT JOIN {Constants.Tables.Genres} AS g ON (gs.GenresId = g.Id)
LEFT JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)
LEFT JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)
WHERE 1=1 {libraryIds.AddLibraryIdFilterAsAnd("s")}";
        }

        public static string GenerateShowPageQuery(this DbSet<SqlShow> shows, Filter[] filters, IEnumerable<string> libraryIds,
            string sortField, string sortOrder)
        {
            var query = shows.GenerateFullShowWithGenresQuery(libraryIds);
            query = filters.Aggregate(query, (current, filter) => current + AddShowFilters(filter));

            if (string.IsNullOrWhiteSpace(sortField))
            {
                return query;
            }

            sortField = sortField.FirstCharToUpper();
            query += $"ORDER BY {sortField} {sortOrder.ToUpper()} ";

            return query;
        }

        private static string AddShowFilters(Filter filter)
        {
            return $"AND {GenerateFilterString(filter)}\n";
        }

        private static string GenerateFilterString(Filter filter)
        {
            switch (filter.Field)
            {
                case "Genres":
                    {
                        return filter.Operation switch
                        {
                            "!any" => GenerateExistsGenreLine($"g0.Name = '{filter.Value}'", true),
                            "any" => GenerateExistsGenreLine($"g0.Name = '{filter.Value}'"),
                            _ => string.Empty
                        };
                    }
                case "Images":
                    return filter.Value switch
                    {
                        "Primary" => filter.Operation switch
                        {
                            "!null" => $"s.Primary IS NOT NULL",
                            "null" => $"s.Primary IS NOT NULL",
                            _ => string.Empty
                        },
                        "Logo" => filter.Operation switch
                        {
                            "!null" => $"s.Logo IS NOT NULL",
                            "null" => $"s.Logo IS NOT NULL",
                            _ => string.Empty
                        },
                        _ => string.Empty
                    };
                case "CommunityRating":
                    var ratingValues = filter.Value.FormatInputValue();
                    return filter.Operation switch
                    {
                        "==" => $"s.CommunityRating = {filter.Value}",
                        "between" => $"s.CommunityRating > {ratingValues[0]} s.CommunityRating < {ratingValues[1]}",
                        _ => string.Empty
                    };
                case "RunTimeTicks":
                    var runTimeValues = filter.Value.FormatInputValue();
                    return filter.Operation switch
                    {
                        "<" => $"s.RunTimeTicks < {filter.Value}",
                        ">" => $"s.RunTimeTicks > {filter.Value}",
                        "between" =>
                            $"s.RunTimeTicks > {runTimeValues[0]} s.RunTimeTicks < {runTimeValues[1]}",
                        _ => string.Empty
                    };
                case "Name":
                    return filter.Operation switch
                    {
                        "==" => $"(s.SortName = '{filter.Value}' OR s.Name = '{filter.Value}')",
                        "contains" => $"(s.SortName LIKE '%{filter.Value}%' OR s.Name LIKE '%{filter.Value}%')",
                        "!contains" =>
                            $"(s.SortName NOT LIKE '%{filter.Value}%' OR s.Name NOT LIKE '%{filter.Value}%')",
                        "startsWith" => $"(s.SortName LIKE '{filter.Value}%' OR s.Name LIKE '{filter.Value}%')",
                        "endsWith" => $"(s.SortName LIKE '%{filter.Value}' OR s.Name LIKE '%{filter.Value}')",
                        _ => string.Empty
                    };
                default:
                    return filter.Operation switch
                    {
                        "==" => $"s.{filter.Field} = '{filter.Value}'",
                        "!=" => $"s.{filter.Field} != '{filter.Value}'",
                        "contains" => $"s.{filter.Field} LIKE '%{filter.Value}%'",
                        "!contains" => $"s.{filter.Field} NOT LIKE '%{filter.Value}%'",
                        "startsWith" => $"s.{filter.Field} LIKE '{filter.Value}%')",
                        "endsWith" => $"s.{filter.Field} LIKE '%{filter.Value}')",
                        "null" => $"s.{filter.Field} IS NULL OR s.{filter.Field} = ''",
                        _ => string.Empty
                    };
            }
        }

        private static string GenerateExistsGenreLine(string query, bool invert = false)
        {
            var prefix = invert ? "NOT " : string.Empty;
            return $"{prefix}EXISTS (SELECT 1 FROM {Constants.Tables.GenreMovie} AS s0 INNER JOIN {Constants.Tables.Genres} AS g0 ON s0.GenresId = g0.Id WHERE (s.Id = s0.GenresId) AND ({query}))";
        }
    }
}
