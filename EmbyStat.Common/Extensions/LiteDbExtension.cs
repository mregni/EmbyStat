using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Movies;
using LiteDB;

namespace EmbyStat.Common.Extensions
{
    public static class LiteDbExtension
    {
        public static ILiteQueryable<T> FilterOnLibrary<T>(this ILiteQueryable<T> query, IReadOnlyList<string> libraryIds) where T : Media
        {
            if (libraryIds.Any())
            {
                query = query.Where(x => libraryIds.Any(y => y == x.CollectionId));
            }

            return query;
        }

        public static IQueryable<T> FilterOnLibrary<T>(this IQueryable<T> query, IReadOnlyList<string> libraryIds) where T : SqlMedia
        {
            if (libraryIds.Any())
            {
                query = query.Where(x => libraryIds.Any(y => y == x.CollectionId));
            }

            return query;
        }

        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Filter[] filters) where T : SqlMovie
        {
            return filters.Aggregate(query, (current, filter) => current.ApplyFilter(filter));
        }

        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Filter filter) where T : SqlMovie
        {
            switch (filter.Field)
            {
                case "PremiereDate":
                    var values = Array.Empty<DateTime>();
                    if (filter.Operation != "null")
                    {
                        values = FormatDateInputValue(filter.Value);
                    }

                    return filter.Operation switch
                    {
                        "==" => query.Where(x => (x.PremiereDate ?? DateTime.MinValue) == values[0]),
                        "<" => query.Where(x => (x.PremiereDate ?? DateTime.MaxValue) < values[0]),
                        ">" => query.Where(x => (x.PremiereDate ?? DateTime.MinValue) > values[0]),
                        "between" => query.Where(x => (x.PremiereDate ?? DateTime.MinValue) > values[0] && (x.PremiereDate ?? DateTime.MinValue) < values[1]),
                        "null" => query.Where(x => x.PremiereDate == null),
                        _ => query
                    };
                case "Genres":
                    return filter.Operation switch
                    {
                        "!any" => query.Where(x => x.Genres.All(y => y.Name != filter.Value)),
                        "any" => query.Where(x => x.Genres.Any(y => y.Name == filter.Value)),
                        _ => query
                    };
                case "Images":
                    return filter.Value switch
                    {
                        "Primary" => filter.Operation switch
                        {
                            "!null" => query.Where(x => !string.IsNullOrWhiteSpace(x.Primary)),
                            "null" => query.Where(x => string.IsNullOrWhiteSpace(x.Primary)),
                            _ => query
                        },
                        "Logo" => filter.Operation switch
                        {
                            "!null" => query.Where(x => !string.IsNullOrWhiteSpace(x.Primary)),
                            "null" => query.Where(x => string.IsNullOrWhiteSpace(x.Primary)),
                            _ => query
                        },
                        _ => query
                    };
                case "CommunityRating":
                    return filter.Operation switch
                    {
                        "==" => query.Where(x => (x.CommunityRating ?? 0M) == Convert.ToDecimal(filter.Value)),
                        "between" => query.Where(x => (x.CommunityRating ?? 0M) > FormatInputValue(filter.Value)[0]
                                                      && (x.CommunityRating ?? 0M) < FormatInputValue(filter.Value)[1]),
                        _ => query
                    };
                case "RunTimeTicks":
                    return filter.Operation switch
                    {
                        "<" => query.Where(x => (x.RunTimeTicks ?? 0) < Convert.ToInt64(filter.Value)),
                        ">" => query.Where(x => (x.RunTimeTicks ?? 0) > Convert.ToInt64(filter.Value)),
                        "between" => query.Where(x => (x.RunTimeTicks ?? 0) > FormatInputValue(filter.Value)[0]
                                                      && (x.RunTimeTicks ?? 0) < FormatInputValue(filter.Value)[1]),
                        _ => query
                    };
                default:
                    return filter.Operation switch
                    {
                        "==" => query.Where(x => (string)(typeof(SqlMovie).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty) == filter.Value),
                        "!=" => query.Where(x => ((string)typeof(SqlMovie).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty) != filter.Value),
                        "contains" => query.Where(x => ((string)typeof(SqlMovie).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty).ToLowerInvariant().Contains(filter.Value.ToLowerInvariant())),
                        "!contains" => query.Where(x => !((string)typeof(SqlMovie).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty).ToLowerInvariant().Contains(filter.Value.ToLowerInvariant())),
                        "startsWith" => query.Where(x => ((string)typeof(SqlMovie).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty).ToLowerInvariant().StartsWith(filter.Value.ToLowerInvariant())),
                        "endsWith" => query.Where(x => ((string)typeof(SqlMovie).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty).ToLowerInvariant().EndsWith(filter.Value.ToLowerInvariant())),
                        "null" => query.Where(x => typeof(SqlMovie).GetProperty(filter.Field).GetValue(x, null) == null),
                        _ => query,
                    };
            }
        }

        private static decimal[] FormatInputValue(string value)
        {
            return FormatInputValue(value, 1);
        }

        private static decimal[] FormatInputValue(string value, int multiplier)
        {
            var decodedValue = HttpUtility.UrlDecode(value);
            if (decodedValue.Contains('|'))
            {
                var left = Convert.ToDecimal(decodedValue.Split('|')[0]) * multiplier;
                var right = Convert.ToDecimal(decodedValue.Split('|')[1]) * multiplier;

                //switching sides if user put the biggest number on the left side.
                if (right < left)
                {
                    (left, right) = (right, left);
                }

                return new[] { left, right };
            }

            if (decodedValue.Length == 0)
            {
                return new[] { 0M };
            }

            return new[] { Convert.ToDecimal(decodedValue) * multiplier };
        }

        private static DateTime[] FormatDateInputValue(string value)
        {
            if (value.Contains('|'))
            {
                var left = DateTime.Parse(value.Split('|')[0]);
                var right = DateTime.Parse(value.Split('|')[1]);

                //switching sides if user put the biggest number on the left side.
                if (right < left)
                {
                    (left, right) = (right, left);
                }

                return new[] { left, right };
            }
            else
            {
                return new[] { DateTime.Parse(value) };
            }
        }
    }
}
