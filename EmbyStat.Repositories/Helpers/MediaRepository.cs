using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Query;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Repositories.Interfaces.Helpers;
using LiteDB;
using MediaBrowser.Model.Extensions;

namespace EmbyStat.Repositories.Helpers
{
    public abstract class MediaRepository<T> : BaseRepository, IMediaRepository<T> where T : Extra
    {
        protected MediaRepository(IDbContext context) : base(context)
        {

        }

        public IEnumerable<T> GetNewestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            return GetWorkingLibrarySet(collection, libraryIds)
                .Where(x => x.PremiereDate != null)
                .OrderByDescending(x => x.PremiereDate)
                .Take(count);
        }

        public IEnumerable<T> GetOldestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            return GetWorkingLibrarySet(collection, libraryIds)
                .Where(x => x.PremiereDate != null)
                .OrderBy(x => x.PremiereDate)
                .Take(count);
        }

        public IEnumerable<T> GetHighestRatedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            return GetWorkingLibrarySet(collection, libraryIds)
                .Where(x => x.CommunityRating != null)
                .OrderByDescending(x => x.CommunityRating)
                .Take(count);
        }

        public IEnumerable<T> GetLowestRatedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            return GetWorkingLibrarySet(collection, libraryIds)
                .Where(x => x.CommunityRating != null)
                .OrderBy(x => x.CommunityRating)
                .Take(count);
        }

        public IEnumerable<T> GetLatestAddedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();

            return GetWorkingLibrarySet(collection, libraryIds)
                .OrderByDescending(x => x.DateCreated)
                .Take(count);
        }

        public virtual int GetMediaCount(Filter[] filters, IReadOnlyList<string> libraryIds)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            var query = GetWorkingLibrarySet(collection, libraryIds);
            foreach (var filter in filters)
            {
                query = ApplyFilter(query, filter);
            }

            return query.Count();
        }

        public int GetMediaCount(IReadOnlyList<string> libraryIds)
        {
            return GetMediaCount(Array.Empty<Filter>(), libraryIds);
        }

        public bool Any()
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            return collection.Exists(Query.All());
        }

        public int GetMediaCountForPerson(string name, string genre)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            return collection
                .FindAll()
                .Where(x => x.Genres.Any(y => y == genre))
                .Count(x => x.People.Any(y => name == y.Name));
        }

        public int GetMediaCountForPerson(string name)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            return collection
                .FindAll()
                .Count(x => x.People.Any(y => name == y.Name));
        }

        public int GetGenreCount(IReadOnlyList<string> libraryIds)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            var genres = GetWorkingLibrarySet(collection, libraryIds)
                .Select(x => x.Genres);

            return genres.SelectMany(x => x)
                .Distinct()
                .Count();
        }

        #region People

        public int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();

            return GetWorkingLibrarySet(collection, libraryIds)
                .SelectMany(x => x.People)
                .DistinctBy(x => x.Id)
                .Count(x => x.Type == type);
        }

        public IEnumerable<string> GetMostFeaturedPersons(IReadOnlyList<string> libraryIds, PersonType type, int count)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();

            return GetWorkingLibrarySet(collection, libraryIds)
                .SelectMany(x => x.People)
                .Where(x => x.Type == type)
                .GroupBy(x => x.Name, (name, people) => new { Name = name, Count = people.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Name)
                .Take(count);
        }

        #endregion

        #region Filters

        public IEnumerable<LabelValuePair> CalculateGenreFilterValues(IReadOnlyList<string> libraryIds)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            var query = GetWorkingLibrarySet(collection, libraryIds);
            return query
                .SelectMany(x => x.Genres)
                .Select(x => new LabelValuePair { Value = x, Label = x })
                .DistinctBy(x => x.Label)
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateCollectionFilterValues()
        {
            //TODO: safe collections somewhere so we can display names in the dropdown
            //not working at the moment, will display Id's
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<T>();
            var query = collection.FindAll();
            return query
                .Select(x => new LabelValuePair { Value = x.CollectionId, Label = x.CollectionId })
                .DistinctBy(x => x.Label)
                .OrderBy(x => x.Label);
        }

        protected IEnumerable<T> ApplyFilter(IEnumerable<T> query, Filter filter)
        {
            switch (filter.Field)
            {
                case "PremiereDate":
                    var values = Array.Empty<DateTime>();
                    if (filter.Operation != "null")
                    {
                        values = FormatDateInputValue(filter.Value);
                    }

                    return (filter.Operation switch
                    {
                        "==" => query.Where(x => ((DateTime?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? DateTime.MinValue) == values[0]),
                        "<" => query.Where(x => ((DateTime?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? DateTime.MaxValue) < values[0]),
                        ">" => query.Where(x => ((DateTime?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? DateTime.MinValue) > values[0]),
                        "between" => query.Where(x => ((DateTime?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? DateTime.MinValue) > values[0]
                                                      && ((DateTime?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? DateTime.MinValue) < values[1]),
                        "null" => query.Where(x => (DateTime?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) == null),
                        _ => query
                    });
                case "Genres":
                    return (filter.Operation switch
                    {
                        "!any" => query.Where(x => x.Genres.All(y => y != filter.Value)),
                        "any" => query.Where(x => x.Genres.Any(y => y == filter.Value)),
                        _ => query
                    });
                case "Images":
                    return (filter.Operation switch
                    {
                        "!null" => query.Where(x => ((string)typeof(T).GetProperty(filter.Value)?.GetValue(x, null) ?? string.Empty).ToLowerInvariant().Contains(filter.Value.ToLowerInvariant())),
                        "null" => query.Where(x => string.IsNullOrWhiteSpace((string)typeof(T).GetProperty(filter.Value)?.GetValue(x, null) ?? string.Empty)),
                        _ => query
                    });
                case "CommunityRating":
                    return (filter.Operation switch
                    {
                        "==" => query.Where(x => ((float?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? 0d) == Convert.ToDouble(filter.Value)),
                        "between" => query.Where(x => ((float?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? 0d) > FormatInputValue(filter.Value)[0]
                                                      && ((float?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? 0d) < FormatInputValue(filter.Value)[1]),
                        _ => query
                    });
                case "RunTimeTicks":
                    return (filter.Operation switch
                    {
                        "<" => query.Where(x => ((long?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? 0) < Convert.ToInt64(filter.Value)),
                        ">" => query.Where(x => ((long?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? 0) > Convert.ToInt64(filter.Value)),
                        "between" => query.Where(x => ((long?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? 0) > FormatInputValue(filter.Value)[0]
                                                      && ((long?)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? 0) < FormatInputValue(filter.Value)[1]),
                        _ => query
                    });
                default:
                    return (filter.Operation switch
                    {
                        "==" => query.Where(x => (string)(typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? string.Empty) == filter.Value),
                        "!=" => query.Where(x => ((string)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? string.Empty) != filter.Value),
                        "contains" => query.Where(x => ((string)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? string.Empty).ToLowerInvariant().Contains(filter.Value.ToLowerInvariant())),
                        "!contains" => query.Where(x => !((string)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? string.Empty).ToLowerInvariant().Contains(filter.Value.ToLowerInvariant())),
                        "startsWith" => query.Where(x => ((string)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? string.Empty).ToLowerInvariant().StartsWith(filter.Value.ToLowerInvariant())),
                        "endsWith" => query.Where(x => ((string)typeof(T).GetProperty(filter.Field)?.GetValue(x, null) ?? string.Empty).ToLowerInvariant().EndsWith(filter.Value.ToLowerInvariant())),
                        "null" => query.Where(x => typeof(T).GetProperty(filter.Field)?.GetValue(x, null) == null),
                        _ => query,
                    });
            }
        }

        protected double[] FormatInputValue(string value, int multiplier = 1)
        {
            var decodedValue = HttpUtility.UrlDecode(value);
            if (decodedValue.Contains('|'))
            {
                var left = Convert.ToDouble(decodedValue.Split('|')[0]) * multiplier;
                var right = Convert.ToDouble(decodedValue.Split('|')[1]) * multiplier;

                //switching sides if user put the biggest number on the left side.
                if (right < left)
                {
                    var temp = left;
                    left = right;
                    right = temp;
                }

                return new[] { left, right };
            }

            if (decodedValue.Length == 0)
            {
                return new[] { 0d };
            }

            return new[] { Convert.ToDouble(decodedValue) * multiplier };
        }

        protected DateTime[] FormatDateInputValue(string value)
        {
            if (value.Contains('|'))
            {
                var left = DateTime.Parse(value.Split('|')[0]);
                var right = DateTime.Parse(value.Split('|')[1]);

                //switching sides if user put the biggest number on the left side.
                if (right < left)
                {
                    var temp = left;
                    left = right;
                    right = temp;
                }

                return new[] { left, right };
            }
            else
            {
                return new[] { DateTime.Parse(value) };
            }
        }

        #endregion
    }
}
