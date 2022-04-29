namespace EmbyStat.Repositories.Helpers;

//public abstract class MediaRepository<T> : BaseRepository, IMediaRepository<T> where T : Extra
//{
//    protected MediaRepository(IDbContext context) : base(context)
//    {

//    }

//    public IEnumerable<T> GetNewestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
//    {
//        using var database = Context.CreateDatabaseContext();
//        var ids = database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds)
//            .Where(x => x.PremiereDate.HasValue)
//            .OrderByDescending(x => x.PremiereDate)
//            .Select(x => x.Id)
//            .Limit(count)
//            .ToEnumerable();

//        return database.GetCollection<T>()
//            .Query()
//            .Where(x => ids.Any(y => y == x.Id))
//            .ToEnumerable();
//    }

//    public IEnumerable<T> GetOldestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
//    {
//        using var database = Context.CreateDatabaseContext();
//        var ids = database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds)
//            .Where(x => x.PremiereDate.HasValue)
//            .OrderBy(x => x.PremiereDate)
//            .Select(x => x.Id)
//            .Limit(count)
//            .ToEnumerable();

//        return database.GetCollection<T>()
//            .Query()
//            .Where(x => ids.Any(y => y == x.Id))
//            .ToEnumerable();
//    }

//    public IEnumerable<T> GetHighestRatedMedia(IReadOnlyList<string> libraryIds, int count)
//    {
//        using var database = Context.CreateDatabaseContext();
//        return database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds)
//            .Where(x => x.CommunityRating != null)
//            .OrderByDescending(x => x.CommunityRating)
//            .Limit(count)
//            .ToEnumerable();
//    }

//    public IEnumerable<T> GetLowestRatedMedia(IReadOnlyList<string> libraryIds, int count)
//    {
//        using var database = Context.CreateDatabaseContext();
//        var ids = database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds)
//            .Where(x => x.CommunityRating != null)
//            .OrderBy(x => x.CommunityRating)
//            .Select(x => x.Id)
//            .Limit(count)
//            .ToEnumerable();

//        return database.GetCollection<T>()
//            .Query()
//            .Where(x => ids.Any(y => y == x.Id))
//            .ToEnumerable();
//    }

//    public IEnumerable<T> GetLatestAddedMedia(IReadOnlyList<string> libraryIds, int count)
//    {
//        using var database = Context.CreateDatabaseContext();
//        var ids = database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds)
//            .OrderByDescending(x => x.DateCreated)
//            .Select(x => x.Id)
//            .Limit(count)
//            .ToEnumerable();

//        return database.GetCollection<T>()
//            .Query()
//            .Where(x => ids.Any(y => y == x.Id))
//            .ToEnumerable();
//    }

//    public virtual int GetMediaCount(Filter[] filters, IReadOnlyList<string> libraryIds)
//    {
//        using var database = Context.CreateDatabaseContext();
//        var query = database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds);
//        query = filters.Aggregate(query, ApplyFilter);
//        return query.Count();
//    }

//    public int GetMediaCount(IReadOnlyList<string> libraryIds)
//    {
//        return GetMediaCount(Array.Empty<Filter>(), libraryIds);
//    }

//    public bool Any()
//    {
//        using var database = Context.CreateDatabaseContext();
//        var collection = database.GetCollection<T>();
//        return collection.Exists(Query.All());
//    }

//    public int GetMediaCountForPerson(string name, string genre)
//    {
//        using var database = Context.CreateDatabaseContext();
//        return database.GetCollection<T>()
//            .Query()
//            .Where(x => x.Genres.Any(y => y == genre) && x.People.Any(y => name == y.Name))
//            .Count();
//    }

//    public int GetMediaCountForPerson(string name)
//    {
//        using var database = Context.CreateDatabaseContext();
//        return database.GetCollection<T>()
//            .Query()
//            .Where(x => x.People.Any(y => name == y.Name))
//            .Count();
//    }

//    public int GetGenreCount(IReadOnlyList<string> libraryIds)
//    {
//        using var database = Context.CreateDatabaseContext();
//        return database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds)
//            .Select(x => x.Genres)
//            .ToEnumerable()
//            .SelectMany(x => x)
//            .Distinct()
//            .Count();
//    }

//    #region People

//    public int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type)
//    {
//        using var database = Context.CreateDatabaseContext();
//        return database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds)
//            .ToEnumerable()
//            .SelectMany(x => x.People)
//            .DistinctBy(x => x.Id)
//            .Count(x => x.Type == type);
//    }

//    public IEnumerable<string> GetMostFeaturedPersons(IReadOnlyList<string> libraryIds, PersonType type, int count)
//    {
//        using var database = Context.CreateDatabaseContext();
//        return database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds)
//            .ToEnumerable()
//            .SelectMany(x => x.People)
//            .Where(x => x.Type == type)
//            .GroupBy(x => x.Name, (name, people) => new { Name = name, Count = people.Count() })
//            .OrderByDescending(x => x.Count)
//            .Select(x => x.Name)
//            .Take(count);
//    }

//    #endregion

//    #region Filters

//    public IEnumerable<LabelValuePair> CalculateGenreFilterValues(IReadOnlyList<string> libraryIds)
//    {
//        using var database = Context.CreateDatabaseContext();
//        return database.GetCollection<T>()
//            .Query()
//            .FilterOnLibrary(libraryIds)
//            .ToEnumerable()
//            .SelectMany(x => x.Genres)
//            .Select(x => new LabelValuePair { Value = x, Label = x })
//            .DistinctBy(x => x.Label)
//            .OrderBy(x => x.Label);
//    }

//    public IEnumerable<LabelValuePair> CalculateCollectionFilterValues()
//    {
//        //TODO: safe collections somewhere so we can display names in the dropdown
//        //not working at the moment, will display Id's
//        using var database = Context.CreateDatabaseContext();
//        return database.GetCollection<T>()
//            .Query()
//            .Select(x => new LabelValuePair { Value = x.CollectionId, Label = x.CollectionId })
//            .ToEnumerable()
//            .DistinctBy(x => x.Label)
//            .OrderBy(x => x.Label);
//    }

//    protected ILiteQueryable<T> ApplyFilter(ILiteQueryable<T> query, Filter filter)
//    {
//        switch (filter.Field)
//        {
//            case "PremiereDate":
//                var values = Array.Empty<DateTimeUtc>();
//                if (filter.Operation != "null")
//                {
//                    values = FormatDateInputValue(filter.Value);
//                }

//                return filter.Operation switch
//                {
//                    "==" => query.Where(x => (x.PremiereDate ?? DateTimeUtc.MinValue) == values[0]),
//                    "<" => query.Where(x => (x.PremiereDate ?? DateTimeUtc.MaxValue) < values[0]),
//                    ">" => query.Where(x => (x.PremiereDate ?? DateTimeUtc.MinValue) > values[0]),
//                    "between" => query.Where(x => (x.PremiereDate ?? DateTimeUtc.MinValue) > values[0] && (x.PremiereDate ?? DateTimeUtc.MinValue) < values[1]),
//                    "null" => query.Where(x => x.PremiereDate == null),
//                    _ => query
//                };
//            case "Genres":
//                return filter.Operation switch
//                {
//                    "!any" => query.Where(x => x.Genres.All(y => y != filter.Value)),
//                    "any" => query.Where(x => x.Genres.Any(y => y == filter.Value)),
//                    _ => query
//                };
//            case "Images":
//                return filter.Value switch
//                {
//                    "Primary" => filter.Operation switch
//                    {
//                        "!null" => query.Where(x => !string.IsNullOrWhiteSpace(x.Primary)),
//                        "null" => query.Where(x => string.IsNullOrWhiteSpace(x.Primary)),
//                        _ => query
//                    },
//                    "Logo" => filter.Operation switch
//                    {
//                        "!null" => query.Where(x => !string.IsNullOrWhiteSpace(x.Primary)),
//                        "null" => query.Where(x => string.IsNullOrWhiteSpace(x.Primary)),
//                        _ => query
//                    },
//                    _ => query
//                };
//            case "CommunityRating":
//                return filter.Operation switch
//                {
//                    "==" => query.Where(x => (x.CommunityRating ?? 0d) == Convert.ToDouble(filter.Value)),
//                    "between" => query.Where(x => (x.CommunityRating ?? 0d) > FormatInputValue(filter.Value)[0]
//                                                  && (x.CommunityRating ?? 0d) < FormatInputValue(filter.Value)[1]),
//                    _ => query
//                };
//            case "RunTimeTicks":
//                return filter.Operation switch
//                {
//                    "<" => query.Where(x => (x.RunTimeTicks ?? 0) < Convert.ToInt64(filter.Value)),
//                    ">" => query.Where(x => (x.RunTimeTicks ?? 0) > Convert.ToInt64(filter.Value)),
//                    "between" => query.Where(x => (x.RunTimeTicks ?? 0) > FormatInputValue(filter.Value)[0]
//                                                  && (x.RunTimeTicks ?? 0) < FormatInputValue(filter.Value)[1]),
//                    _ => query
//                };
//            default:
//                return filter.Operation switch
//                {
//                    "==" => query.Where(x => (string)(typeof(T).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty) == filter.Value),
//                    "!=" => query.Where(x => ((string)typeof(T).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty) != filter.Value),
//                    "contains" => query.Where(x => ((string)typeof(T).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty).ToLowerInvariant().Contains(filter.Value.ToLowerInvariant())),
//                    "!contains" => query.Where(x => !((string)typeof(T).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty).ToLowerInvariant().Contains(filter.Value.ToLowerInvariant())),
//                    "startsWith" => query.Where(x => ((string)typeof(T).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty).ToLowerInvariant().StartsWith(filter.Value.ToLowerInvariant())),
//                    "endsWith" => query.Where(x => ((string)typeof(T).GetProperty(filter.Field).GetValue(x, null) ?? string.Empty).ToLowerInvariant().EndsWith(filter.Value.ToLowerInvariant())),
//                    "null" => query.Where(x => typeof(T).GetProperty(filter.Field).GetValue(x, null) == null),
//                    _ => query,
//                };
//        }
//    }

//    protected double[] FormatInputValue(string value)
//    {
//        return FormatInputValue(value, 1);
//    }

//    protected double[] FormatInputValue(string value, int multiplier)
//    {
//        var decodedValue = HttpUtility.UrlDecode(value);
//        if (decodedValue.Contains('|'))
//        {
//            var left = Convert.ToDouble(decodedValue.Split('|')[0]) * multiplier;
//            var right = Convert.ToDouble(decodedValue.Split('|')[1]) * multiplier;

//            //switching sides if user put the biggest number on the left side.
//            if (right < left)
//            {
//                (left, right) = (right, left);
//            }

//            return new[] { left, right };
//        }

//        if (decodedValue.Length == 0)
//        {
//            return new[] { 0d };
//        }

//        return new[] { Convert.ToDouble(decodedValue) * multiplier };
//    }

//    protected DateTimeUtc[] FormatDateInputValue(string value)
//    {
//        if (value.Contains('|'))
//        {
//            var left = DateTimeUtc.Parse(value.Split('|')[0]);
//            var right = DateTimeUtc.Parse(value.Split('|')[1]);

//            //switching sides if user put the biggest number on the left side.
//            if (right < left)
//            {
//                (left, right) = (right, left);
//            }

//            return new[] { left, right };
//        }
//        else
//        {
//            return new[] { DateTimeUtc.Parse(value) };
//        }
//    }

//    #endregion
//}