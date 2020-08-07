using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Repositories.Helpers;
using EmbyStat.Repositories.Interfaces;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Repositories
{
    public class MovieRepository : MediaRepository<Movie>, IMovieRepository
    {
        public MovieRepository(IDbContext context) : base(context)
        {
            
        }

        public void UpsertRange(IEnumerable<Movie> movies)
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                collection.Upsert(movies);
            });
        }

        public List<Movie> GetAll(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return GetWorkingLibrarySet(collection, libraryIds).OrderBy(x => x.SortName).ToList();
            });
        }

        public List<Movie> GetAllWithImdbId(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return GetWorkingLibrarySet(collection, libraryIds)
                    .Where(x => x.IMDB != null)
                    .OrderBy(x => x.SortName)
                    .ToList();
            });
        }

        public Movie GetMovieById(string id)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return collection.FindById(id);
            });
        }

        public int GetGenreCount(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                var genres = GetWorkingLibrarySet(collection, libraryIds)
                    .Select(x => x.Genres);

                return genres.SelectMany(x => x)
                    .Distinct()
                    .Count();
            });
        }

        public long GetTotalRuntime(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return GetWorkingLibrarySet(collection, libraryIds)
                    .Select(x => x.RunTimeTicks)
                    .Sum(x => x ?? 0);
            });
        }

        public double GetTotalDiskSize(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return GetWorkingLibrarySet(collection, libraryIds)
                    .Select(x => x.MediaSources.FirstOrDefault())
                    .Sum(x => x?.SizeInMb ?? 0);
            });
        }

        public IEnumerable<Movie> GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks, int count)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return GetWorkingLibrarySet(collection, libraryIds)
                    .Where(x => x.RunTimeTicks != null && x.RunTimeTicks > toShortMovieTicks)
                    .OrderBy(x => x.RunTimeTicks)
                    .Take(count);
            });
        }

        public IEnumerable<Movie> GetLongestMovie(IReadOnlyList<string> libraryIds, int count)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return GetWorkingLibrarySet(collection, libraryIds)
                    .Where(x => x.RunTimeTicks != null)
                    .OrderByDescending(x => x.RunTimeTicks)
                    .Take(count);
            });
        }
        
        #region People

        public int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();

                return GetWorkingLibrarySet(collection, libraryIds)
                    .SelectMany(x => x.People)
                    .DistinctBy(x => x.Id)
                    .Count(x => x.Type == type);
            });
        }

        public IEnumerable<string> GetMostFeaturedPersons(IReadOnlyList<string> libraryIds, PersonType type, int count)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();

                return GetWorkingLibrarySet(collection, libraryIds)
                    .SelectMany(x => x.People)
                    .Where(x => x.Type == type)
                    .GroupBy(x => x.Name, (name, people) => new {Name = name, Count = people.Count()})
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Name)
                    .Take(count);
            });
        }

        #endregion

        #region Suspicious

        public List<Movie> GetToShortMovieList(IReadOnlyList<string> libraryIds, int toShortMovieMinutes)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return GetWorkingLibrarySet(collection, libraryIds)
                    .Where(x => x.RunTimeTicks < TimeSpan.FromMinutes(toShortMovieMinutes).Ticks)
                    .OrderBy(x => x.SortName)
                    .ToList();
            });
        }

        public List<Movie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return GetWorkingLibrarySet(collection, libraryIds)
                    .Where(x => x.IMDB == null)
                    .OrderBy(x => x.SortName)
                    .ToList();
            });
        }

        public List<Movie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                return GetWorkingLibrarySet(collection, libraryIds)
                    .Where(x => x.Primary == null)
                    .OrderBy(x => x.SortName)
                    .ToList();
            });
        }

        public IEnumerable<Movie> GetMoviePage(int skip, int take, string sort, Filter[] filters, List<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                var query = GetWorkingLibrarySet(collection, libraryIds);

                query = filters.Aggregate(query, ApplyMovieFilters);

                if (!string.IsNullOrWhiteSpace(sort))
                {
                    var jObj = JsonConvert.DeserializeObject<JArray>(sort);
                    var selector = jObj[0]["selector"].Value<string>().FirstCharToUpper();
                    var desc = jObj[0]["desc"].Value<bool>();

                    query = desc 
                        ? query.OrderByDescending(x => typeof(Movie).GetProperty(selector)?.GetValue(x, null))
                        : query.OrderBy(x => typeof(Movie).GetProperty(selector)?.GetValue(x, null));
                }

                return query
                    .Skip(skip)
                    .Take(take);
            });
        }

        #endregion

        public void RemoveMovies()
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                collection.DeleteMany("1=1");
            });
        }

        public override int GetMediaCount(Filter[] filters, IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                var query = GetWorkingLibrarySet(collection, libraryIds);
                foreach (var filter in filters)
                {
                    query = ApplyMovieFilters(query, filter);
                }

                return query.Count();
            });
        }

        #region Filters
        public IEnumerable<LabelValuePair> CalculateSubtitleFilterValues(IReadOnlyList<string> libraryIds)
        {
            var re = new Regex(@"\ \([0-9a-zA-Z -_]*\)$");
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                var query = GetWorkingLibrarySet(collection, libraryIds);
                return query
                    .SelectMany(x => x.SubtitleStreams)
                    .Where(x => x.Language != "und" && x.Language != "Und" && x.Language != null)
                    .Select(x => new LabelValuePair {Value = x.Language, Label = re.Replace(x.DisplayTitle, string.Empty) })
                    .DistinctBy(x => x.Label)
                    .OrderBy(x => x.Label);
            });
        }

        public IEnumerable<LabelValuePair> CalculateContainerFilterValues(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Movie>();
                var query = GetWorkingLibrarySet(collection, libraryIds);
                return query
                    .Select(x => new LabelValuePair { Value = x.Container, Label = x.Container })
                    .DistinctBy(x => x.Label)
                    .OrderBy(x => x.Label);
            });
        }

        private IEnumerable<Movie> ApplyMovieFilters(IEnumerable<Movie> query, Filter filter)
        {
            switch (filter.Field)
            {
                case "Container":
                    return (filter.Operation switch
                    {
                        "==" => query.Where(x => x.Container == filter.Value),
                        "!=" => query.Where(x => x.Container != filter.Value),
                        "null" => query.Where(x => string.IsNullOrWhiteSpace(x.Container)),
                        _ => query
                    });
                case "Subtitles":
                    return (filter.Operation switch
                    {
                        "!any" => query.Where(x => x.SubtitleStreams.All(y => y.Language != filter.Value)),
                        "any" => query.Where(x => x.SubtitleStreams.Any(y => y.Language == filter.Value)),
                        "empty" => query.Where(x => x.SubtitleStreams == null || x.SubtitleStreams.Count == 0),
                        _ => query
                    });
                case "SizeInMb":
                    var sizeValues = FormatInputValue(filter.Value, 1024);
                    return (filter.Operation switch
                    {
                        "<" => query.Where(x => x.MediaSources.All(y => y.SizeInMb < sizeValues[0])),
                        ">" => query.Where(x => x.MediaSources.All(y => y.SizeInMb > sizeValues[0])),
                        "null" => query.Where(x => x.MediaSources.All(y => Math.Abs(y.SizeInMb) < 0.1)),
                        "between" => query.Where(x =>
                            x.MediaSources.All(y => y.SizeInMb > sizeValues[0]) &&
                            x.MediaSources.All(y => y.SizeInMb < sizeValues[1])),
                        _ => query
                    });
                case "Height":
                    var heightValues = FormatInputValue(filter.Value);
                    return (filter.Operation switch
                    {
                        "==" => query.Where(x => x.VideoStreams.All(y => y.Height == heightValues[0])),
                        "<" => query.Where(x => x.VideoStreams.All(y => y.Height < heightValues[0])),
                        ">" => query.Where(x => x.VideoStreams.All(y => y.Height > heightValues[0])),
                        "null" => query.Where(x => x.VideoStreams.All(y => y.Height == null)),
                        "between" => query.Where(x =>
                            x.VideoStreams.All(y => y.Height > heightValues[0]) &&
                            x.VideoStreams.All(y => y.Height < heightValues[1])),
                        _ => query
                    });
                case "Width":
                    var heightWidths = FormatInputValue(filter.Value);
                    return (filter.Operation switch
                    {
                        "==" => query.Where(x => x.VideoStreams.All(y => y.Width == heightWidths[0])),
                        "<" => query.Where(x => x.VideoStreams.All(y => y.Width < heightWidths[0])),
                        ">" => query.Where(x => x.VideoStreams.All(y => y.Width > heightWidths[0])),
                        "null" => query.Where(x => x.VideoStreams.All(y => y.Width == null)),
                        "between" => query.Where(x =>
                            x.VideoStreams.All(y => y.Width > heightWidths[0]) &&
                            x.VideoStreams.All(y => y.Width < heightWidths[1])),
                        _ => query
                    });
                case "AverageFrameRate":
                    var heightFps = FormatInputValue(filter.Value);
                    return (filter.Operation switch
                    {
                        "==" => query.Where(x => x.VideoStreams.All(y => y.AverageFrameRate == heightFps[0])),
                        "<" => query.Where(x => x.VideoStreams.All(y => y.AverageFrameRate < heightFps[0])),
                        ">" => query.Where(x => x.VideoStreams.All(y => y.AverageFrameRate > heightFps[0])),
                        "null" => query.Where(x => x.VideoStreams.All(y => y.AverageFrameRate == null)),
                        "between" => query.Where(x =>
                            x.VideoStreams.All(y => y.AverageFrameRate > heightFps[0]) &&
                            x.VideoStreams.All(y => y.AverageFrameRate < heightFps[1])),
                        _ => query
                    });
                default:
                    return ApplyFilter(query, filter);
            }
        }
        #endregion
    }
}
