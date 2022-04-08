using System.Linq;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Query;
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
        /// <param name="sortField">Sorting results on a certain column</param>
        /// <param name="sortOrder">Sorting order (asc, desc)</param>
        /// <returns>Sqlite query that can query for all movies with its relations</returns>
        public static string GenerateFullMovieQuery(this DbSet<Movie> movies, Filter[] filters,string sortField, string sortOrder)
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
WHERE 1=1 ";

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
        /// <returns>Sqlite query that can query the count of movies</returns>
        public static string GenerateCountQuery(this DbSet<Movie> list, Filter[] filters)
        {
            var query = $@"
SELECT COUNT() AS Count
FROM {Constants.Tables.Movies} as m
WHERE 1=1 
";
            query = filters.Aggregate(query, (current, filter) => current + AddMovieFilters(filter));

            return query;
        }

        private static string AddMovieFilters(Filter filter)
        {
            return $"AND {GenerateFilterString(filter)} ";
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
                        "empty" => GenerateExistsLine(Constants.Tables.SubtitleStreams, "s0.Id IS NOT NULL", true),
                        _ => string.Empty
                    });
                case "SizeInMb":
                    var sizeValues = filter.Value.FormatInputValue(1024);
                    return (filter.Operation switch
                    {
                        "<" => GenerateExistsLine(Constants.Tables.MediaSources, $"s0.SizeInMb < {sizeValues[0]}"),
                        ">" => GenerateExistsLine(Constants.Tables.MediaSources, $"s0.SizeInMb > {sizeValues[0]}"),
                        "null" => GenerateExistsLine(Constants.Tables.MediaSources, $"s0.SizeInMb = 0"),
                        "between" => GenerateExistsLine(Constants.Tables.MediaSources, $"s0.SizeInMb > {sizeValues[0]} AND s0.SizeInMb < {sizeValues[1]}"),
                        _ => string.Empty
                    });
                case "BitDepth":
                    var depthValues = filter.Value.FormatInputValue();
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
                    var heightValues = filter.Value.FormatInputValue();
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
                    var heightWidths = filter.Value.FormatInputValue();
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
                    var heightFps = filter.Value.FormatInputValue();
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
                            "!null" => $"m.[Primary] != ''",
                            "null" => $"m.[Primary] == ''",
                            _ => string.Empty
                        },
                        "Logo" => filter.Operation switch
                        {
                            "!null" => $"m.Logo != ''",
                            "null" => $"m.Logo == ''",
                            _ => string.Empty
                        },
                        _ => string.Empty
                    };
                case "CommunityRating":
                    var ratingValues = filter.Value.FormatInputValue();
                    return $"s.CommunityRating > {ratingValues[0]} AND s.CommunityRating < {ratingValues[1]}";
                case "RunTimeTicks":
                    var runTimeValues = filter.Value.FormatInputValue();
                    return filter.Operation switch
                    {
                        "<" => $"m.RunTimeTicks < {filter.Value}",
                        ">" => $"m.RunTimeTicks > {filter.Value}",
                        "between" =>
                            $"m.RunTimeTicks > {runTimeValues[0]} AND m.RunTimeTicks < {runTimeValues[1]}",
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

        private static string GenerateExistsGenreLine(string query, bool invert = false)
        {
            var prefix = invert ? "NOT " : string.Empty;
            return $"{prefix}EXISTS (SELECT 1 FROM {Constants.Tables.GenreMovie} AS s0 INNER JOIN {Constants.Tables.Genres} AS g0 ON s0.GenresId = g0.Id WHERE (m.Id = s0.MoviesId) AND ({query}))";
        }

        private static string GenerateExistsLine(string table, string query, bool invert = false)
        {
            var prefix = invert ? "NOT " : string.Empty;
            return $"{prefix}EXISTS (SELECT 1 FROM {table} AS s0 WHERE (m.Id = s0.MovieId) AND ({query}))";
        }
    }
}
