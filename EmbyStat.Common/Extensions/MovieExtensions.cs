using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Common.Extensions
{
    public static class MovieExtensions
    {
        /// <summary>
        /// Generates a full blown movie query with all relations LEFT JOINED. 
        /// </summary>
        /// <param name="movies">movie db set</param>
        /// <param name="filters">Filters that need to be applied in the query</param>
        /// <param name="libraryIds">libraries for which the query should filter</param>
        /// <param name="sortField">Sorting results on a certain column</param>
        /// <param name="sortOrder">Sorting order (asc, desc)</param>
        /// <returns>Sqlite query that can query for all movies with its relations</returns>
        public static string GenerateFullMovieQuery(this DbSet<SqlMovie> movies, Filter[] filters, List<string> libraryIds, string sortField, string sortOrder)
        {
            var query = $@"
SELECT m.*, g.*, aus.*, vis.*, sus.*, mes.*
FROM {Constants.Tables.Movies} as m
LEFT JOIN {Constants.Tables.GenreMovie} AS gm ON (gm.MoviesId = m.Id)
LEFT JOIN {Constants.Tables.Genres} AS g ON (gm.GenresId = g.Id)
LEFT JOIN {Constants.Tables.AudioStreams} AS aus ON (aus.MovieId = m.Id)
LEFT JOIN {Constants.Tables.VideoStreams} AS vis ON (vis.MovieId = m.Id)
LEFT JOIN {Constants.Tables.SubtitleStreams} AS sus ON (sus.MovieId = m.Id)
LEFT JOIN {Constants.Tables.MediaSources} AS mes ON (mes.MovieId = m.Id)
WHERE 1=1 {libraryIds.AddLibraryIdFilterAsAnd("m")}
";

            query = filters.Aggregate(query, (current, filter) => current + AddMovieFilters(filter));

            if (string.IsNullOrWhiteSpace(sortField))
            {
                return query;
            }

            sortField = sortField.FirstCharToUpper();
            query += $"ORDER BY {sortField} {sortOrder.ToUpper()} ";

            return query;
        }

        /// <summary>
        /// Generates a COUNT(*) query for the movie table
        /// </summary>
        /// <param name="filters">Filters that need to be applied in the query</param>
        /// <param name="libraryIds">libraries for which the query should filter</param>
        /// <returns>Sqlite query that can query the count of movies</returns>
        public static string GenerateCountQuery(Filter[] filters, IReadOnlyList<string> libraryIds)
        {
            var query = $@"
SELECT COUNT() AS Count
FROM {Constants.Tables.Movies} as m
WHERE 1=1 {libraryIds.AddLibraryIdFilterAsAnd("m")}
";
            query = filters.Aggregate(query, (current, filter) => current + AddMovieFilters(filter));

            return query;
        }

        private static string AddMovieFilters(Filter filter)
        {
            return $"AND {GenerateFilterString(filter)}\n";
        }

        private static string GenerateFilterString(Filter filter)
        {
            switch (filter.Field)
            {
                case "Subtitles":
                    return (filter.Operation switch
                    {

                        "!any" => GenerateExistsLine(Constants.Tables.SubtitleStreams, $"s0.Language = '{filter.Value}'", true),
                        "any" => GenerateExistsLine(Constants.Tables.SubtitleStreams, $"s0.Language = '{filter.Value}'"),
                        "empty" => GenerateExistsLine(Constants.Tables.SubtitleStreams, $"s0.Language IS NULL"),
                        _ => string.Empty
                    });
                case "SizeInMb":
                    var sizeValues = FormatInputValue(filter.Value, 1024);
                    return (filter.Operation switch
                    {
                        "<" => GenerateExistsLine(Constants.Tables.MediaSources, $"s0.SizeInMb < {sizeValues[0]}"),
                        ">" => GenerateExistsLine(Constants.Tables.MediaSources, $"s0.SizeInMb > {sizeValues[0]}"),
                        "null" => GenerateExistsLine(Constants.Tables.MediaSources, $"s0.SizeInMb = 0"),
                        "between" => GenerateExistsLine(Constants.Tables.MediaSources, $"s0.SizeInMb > {sizeValues[0]} AND s0.SizeInMb < {sizeValues[1]}"),
                        _ => string.Empty
                    });
                case "BitDepth":
                    var depthValues = FormatInputValue(filter.Value);
                    return (filter.Operation switch
                    {
                        "<" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.BitDepth < {depthValues[0]}"),
                        ">" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.BitDepth > {depthValues[0]}"),
                        "null" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.BitDepth = 0"),
                        "between" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.BitDepth > {depthValues[0]} AND s0.BitDepth < {depthValues[1]}"),
                        _ => string.Empty
                    });
                case "Codec":
                    return (filter.Operation switch
                    {
                        "!any" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Codec != '{filter.Value}'"),
                        "any" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Codec = '{filter.Value}'"),
                        _ => string.Empty
                    });
                case "VideoRange":
                    return (filter.Operation switch
                    {
                        "!any" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.VideoRange != '{filter.Value}'"),
                        "any" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.VideoRange = '{filter.Value}'"),
                        _ => string.Empty
                    });
                case "Height":
                    var heightValues = FormatInputValue(filter.Value);
                    return (filter.Operation switch
                    {
                        "==" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Height = {heightValues[0]}"),
                        "<" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Height < {heightValues[0]}"),
                        ">" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Height > {heightValues[0]}"),
                        "null" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Height IS NULL"),
                        "between" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Height > {heightValues[0]} AND s0.Height < {heightValues[1]}"),
                        _ => string.Empty
                    });
                case "Width":
                    var heightWidths = FormatInputValue(filter.Value);
                    return (filter.Operation switch
                    {
                        "==" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Width = {heightWidths[0]}"),
                        "<" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Width < {heightWidths[0]}"),
                        ">" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Width > {heightWidths[0]}"),
                        "null" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Width IS NULL"),
                        "between" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.Width > {heightWidths[0]} AND s0.Width < {heightWidths[1]}"),
                        _ => string.Empty
                    });
                case "AverageFrameRate":
                    var heightFps = FormatInputValue(filter.Value);
                    return (filter.Operation switch
                    {
                        "==" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.AverageFrameRate = {heightFps[0]}"),
                        "<" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.AverageFrameRate < {heightFps[0]}"),
                        ">" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.AverageFrameRate > {heightFps[0]}"),
                        "null" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.AverageFrameRate IS NULL"),
                        "between" => GenerateExistsLine(Constants.Tables.VideoStreams, $"s0.AverageFrameRate > {heightFps[0]} AND s0.AverageFrameRate < {heightFps[1]}"),
                        _ => string.Empty
                    });
                case "Genres":
                    {
                        return filter.Operation switch
                        {
                            "!any" => GenerateExistsGenreLine(Constants.Tables.GenreMovie, Constants.Tables.Genres, $"g0.Name = '{filter.Value}'", true),
                            "any" => GenerateExistsGenreLine(Constants.Tables.GenreMovie, Constants.Tables.Genres, $"g0.Name = '{filter.Value}'"),
                            _ => string.Empty
                        };
                    }
                case "Images":
                    return filter.Value switch
                    {
                        "Primary" => filter.Operation switch
                        {
                            "!null" => $"m.Primary IS NOT NULL",
                            "null" => $"m.Primary IS NOT NULL",
                            _ => string.Empty
                        },
                        "Logo" => filter.Operation switch
                        {
                            "!null" => $"m.Logo IS NOT NULL",
                            "null" => $"m.Logo IS NOT NULL",
                            _ => string.Empty
                        },
                        _ => string.Empty
                    };
                case "CommunityRating":
                    var ratingValues = FormatInputValue(filter.Value);
                    return filter.Operation switch
                    {
                        "==" => $"m.CommunityRating = {filter.Value}",
                        "between" => $"m.CommunityRating > {ratingValues[0]} m.CommunityRating < {ratingValues[1]}",
                        _ => string.Empty
                    };
                case "RunTimeTicks":
                    var runTimeValues = FormatInputValue(filter.Value);
                    return filter.Operation switch
                    {
                        "<" => $"m.RunTimeTicks < {filter.Value}",
                        ">" => $"m.RunTimeTicks > {filter.Value}",
                        "between" =>
                            $"m.RunTimeTicks > {runTimeValues[0]} m.RunTimeTicks < {runTimeValues[1]}",
                        _ => string.Empty
                    };
                case "Name":
                    return filter.Operation switch
                    {
                        "==" => $"(m.SortName = '{filter.Value}' OR m.Name = '{filter.Value}')",
                        "contains" => $"(m.SortName LIKE '%{filter.Value}%' OR m.Name LIKE '%{filter.Value}%')",
                        "!contains" => $"(m.SortName NOT LIKE '%{filter.Value}%' OR m.Name NOT LIKE '%{filter.Value}%')",
                        "startsWith" => $"(m.SortName LIKE '{filter.Value}%' OR m.Name LIKE '{filter.Value}%')",
                        "endsWith" => $"(m.SortName LIKE '%{filter.Value}' OR m.Name LIKE '%{filter.Value}')",
                        _ => string.Empty
                    };
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

        public static double[] FormatInputValue(string value)
        {
            return FormatInputValue(value, 1);
        }

        public static double[] FormatInputValue(string value, int multiplier)
        {
            var decodedValue = HttpUtility.UrlDecode(value);
            if (decodedValue.Contains('|'))
            {
                var left = Convert.ToDouble(decodedValue.Split('|')[0]) * multiplier;
                var right = Convert.ToDouble(decodedValue.Split('|')[1]) * multiplier;

                //switching sides if user put the biggest number on the left side.
                if (right < left)
                {
                    (left, right) = (right, left);
                }

                return new[] { left, right };
            }

            if (decodedValue.Length == 0)
            {
                return new[] { 0d };
            }

            return new[] { Convert.ToDouble(decodedValue) * multiplier };
        }

        private static string GenerateExistsGenreLine(string tableLeft, string tableRight, string query, bool invert = false)
        {
            var prefix = invert ? "NOT " : string.Empty;
            return $"{prefix}EXISTS (SELECT 1 FROM {tableLeft} AS s0 INNER JOIN {tableRight} AS g0 ON s0.GenresId = g0.Id WHERE (m.Id = s0.MoviesId) AND ({query}))";
        }

        private static string GenerateExistsLine(string table, string query, bool invert = false)
        {
            var prefix = invert ? "NOT " : string.Empty;
            return $"{prefix}EXISTS (SELECT 1 FROM {table} AS s0 WHERE (m.Id = s0.MovieId) AND ({query}))";
        }
    }
}
