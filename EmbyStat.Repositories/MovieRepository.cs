//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.RegularExpressions;
//using EmbyStat.Common.Extensions;
//using EmbyStat.Common.Models;
//using EmbyStat.Common.Models.Entities;
//using EmbyStat.Common.Models.Query;
//using EmbyStat.Repositories.Helpers;
//using EmbyStat.Repositories.Interfaces;
//using LiteDB;
//using MoreLinq;

//namespace EmbyStat.Repositories
//{
//    public class MovieRepository : MediaRepository<Movie>, IMovieRepository
//    {
//        public MovieRepository(IDbContext context) : base(context)
//        {

//        }

//        public void UpsertRange(IEnumerable<Movie> movies)
//        {
//            using var database = Context.CreateDatabaseContext();
//            var collection = database.GetCollection<Movie>();
//            collection.Upsert(movies);
//        }

//        public IEnumerable<Movie> GetAll(IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .OrderBy(x => x.SortName)
//                .ToEnumerable();
//        }

//        public IEnumerable<Movie> GetAllWithImdbId(IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Where(x => x.IMDB != null)
//                .OrderBy(x => x.SortName)
//                .ToEnumerable();
//        }

//        public Movie GetById(string id)
//        {
//            using var database = Context.CreateDatabaseContext();
//            var collection = database.GetCollection<Movie>();
//            return collection.FindById(id);
//        }

//        public long GetTotalRuntime(IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Select(x => x.RunTimeTicks)
//                .ToEnumerable()
//                .Sum(x => x ?? 0);
//        }

//        public double GetTotalDiskSpace(IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Select(x => x.MediaSources.FirstOrDefault())
//                .ToEnumerable()
//                .Sum(x => x?.SizeInMb ?? 0);
//        }

//        public IEnumerable<Movie> GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks, int count)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Where(x => x.RunTimeTicks != null && x.RunTimeTicks > toShortMovieTicks)
//                .OrderBy(x => x.RunTimeTicks)
//                .Limit(count)
//                .ToEnumerable();
//        }

//        public IEnumerable<Movie> GetLongestMovie(IReadOnlyList<string> libraryIds, int count)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Where(x => x.RunTimeTicks != null)
//                .OrderByDescending(x => x.RunTimeTicks)
//                .Limit(count)
//                .ToEnumerable();
//        }

//        #region Suspicious

//        public IEnumerable<Movie> GetToShortMovieList(IReadOnlyList<string> libraryIds, int toShortMovieMinutes)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Where(x => x.RunTimeTicks < TimeSpan.FromMinutes(toShortMovieMinutes).Ticks)
//                .OrderBy(x => x.SortName)
//                .ToEnumerable();
//        }

//        public IEnumerable<Movie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Where(x => x.IMDB == null)
//                .OrderBy(x => x.SortName)
//                .ToEnumerable();
//        }

//        public IEnumerable<Movie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Where(x => x.Primary == null)
//                .OrderBy(x => x.SortName)
//                .ToEnumerable();
//        }

//        public IEnumerable<Movie> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters, List<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            var query = database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds);

//            query = filters.Aggregate(query, ApplyMovieFilters);

//            if (!string.IsNullOrWhiteSpace(sortField))
//            {
//                sortField = sortField.FirstCharToUpper();
//                query = sortOrder == "desc"
//                    ? query.OrderByDescending(x => typeof(Movie).GetProperty(sortField).GetValue(x, null))
//                    : query.OrderBy(x => typeof(Movie).GetProperty(sortField).GetValue(x, null));
//            }

//            return query
//                .Skip(skip)
//                .Limit(take)
//                .ToEnumerable();
//        }

//        #endregion

//        public void RemoveAll()
//        {
//            using var database = Context.CreateDatabaseContext();
//            var collection = database.GetCollection<Movie>();
//            collection.DeleteMany("1=1");
//        }

//        public override int Count(Filter[] filters, IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            var query = database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds);
//            query = filters.Aggregate(query, ApplyMovieFilters);
//            return query.Count();
//        }

//        #region Filters
//        public IEnumerable<LabelValuePair> CalculateSubtitleFilterValues(IReadOnlyList<string> libraryIds)
//        {
//            var re = new Regex(@"\ \([0-9a-zA-Z -_]*\)$");
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .ToEnumerable()
//                .SelectMany(x => x.SubtitleStreams)
//                .Where(x => x.Language != "und" && x.Language != "Und" && x.Language != null)
//                .Select(x => new LabelValuePair { Value = x.Language, Label = re.Replace(x.DisplayTitle, string.Empty) })
//                .DistinctBy(x => x.Label)
//                .OrderBy(x => x.Label);
//        }

//        public IEnumerable<LabelValuePair> CalculateContainerFilterValues(IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Select(x => new LabelValuePair { Value = x.Container, Label = x.Container })
//                .ToEnumerable()
//                .DistinctBy(x => x.Label)
//                .OrderBy(x => x.Label);
//        }

//        public IEnumerable<LabelValuePair> CalculateCodecFilterValues(IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Where(x => x.VideoStreams.Any())
//                .Select(x => new LabelValuePair { Value = x.VideoStreams.First().Codec ?? string.Empty, Label = x.VideoStreams.First().Codec ?? string.Empty })
//                .ToEnumerable()
//                .DistinctBy(x => x.Label)
//                .OrderBy(x => x.Label);
//        }

//        public IEnumerable<LabelValuePair> CalculateVideoRangeFilterValues(IReadOnlyList<string> libraryIds)
//        {
//            using var database = Context.CreateDatabaseContext();
//            return database.GetCollection<Movie>()
//                .Query()
//                .FilterOnLibrary(libraryIds)
//                .Where(x => x.VideoStreams.Any())
//                .Select(x => new LabelValuePair { Value = x.VideoStreams.First().VideoRange ?? string.Empty, Label = x.VideoStreams.First().VideoRange ?? string.Empty })
//                .ToEnumerable()
//                .DistinctBy(x => x.Label)
//                .OrderBy(x => x.Label);
//        }

//        private ILiteQueryable<Movie> ApplyMovieFilters(ILiteQueryable<Movie> query, Filter filter)
//        {
//            switch (filter.Field)
//            {
//                case "Container":
//                    return (filter.Operation switch
//                    {
//                        "==" => query.Where(x => x.Container == filter.Value),
//                        "!=" => query.Where(x => x.Container != filter.Value),
//                        "null" => query.Where(x => string.IsNullOrWhiteSpace(x.Container)),
//                        _ => query
//                    });
//                case "Subtitles":
//                    return (filter.Operation switch
//                    {
//                        "!any" => query.Where(x => x.SubtitleStreams.All(y => y.Language != filter.Value)),
//                        "any" => query.Where(x => x.SubtitleStreams.Any(y => y.Language == filter.Value)),
//                        "empty" => query.Where(x => x.SubtitleStreams == null || x.SubtitleStreams.Count == 0),
//                        _ => query
//                    });
//                case "SizeInMb":
//                    var sizeValues = FormatInputValue(filter.Value, 1024);
//                    return (filter.Operation switch
//                    {
//                        "<" => query.Where(x => x.MediaSources.All(y => y.SizeInMb < sizeValues[0])),
//                        ">" => query.Where(x => x.MediaSources.All(y => y.SizeInMb > sizeValues[0])),
//                        "null" => query.Where(x => x.MediaSources.All(y => Math.Abs(y.SizeInMb) < 0.1)),
//                        "between" => query.Where(x =>
//                            x.MediaSources.All(y => y.SizeInMb > sizeValues[0]) &&
//                            x.MediaSources.All(y => y.SizeInMb < sizeValues[1])),
//                        _ => query
//                    });
//                case "BitDepth":
//                    var depthValues = FormatInputValue(filter.Value);
//                    return (filter.Operation switch
//                    {
//                        "<" => query.Where(x => x.VideoStreams.Any() && x.VideoStreams.All(y => y.BitDepth < depthValues[0])),
//                        ">" => query.Where(x => x.VideoStreams.Any() && x.VideoStreams.All(y => y.BitDepth > depthValues[0])),
//                        "null" => query.Where(x => x.VideoStreams.Any() && x.VideoStreams.All(y => Math.Abs(y.BitDepth ?? 0) < 0.1)),
//                        "between" => query.Where(x =>
//                            x.VideoStreams.Any() &&
//                            x.VideoStreams.All(y => y.BitDepth > depthValues[0]) &&
//                            x.VideoStreams.All(y => y.BitDepth < depthValues[1])),
//                        _ => query
//                    });
//                case "Codec":
//                    return (filter.Operation switch
//                    {
//                        "!any" => query.Where(x => x.VideoStreams.Any() && x.VideoStreams.Any(y => y.Codec != filter.Value)),
//                        "any" => query.Where(x => x.VideoStreams.Any() && x.VideoStreams.Any(y => y.Codec == filter.Value)),
//                        _ => query
//                    });
//                case "VideoRange":
//                    return (filter.Operation switch
//                    {
//                        "!any" => query.Where(x => x.VideoStreams.Any() && x.VideoStreams.Any(y => y.VideoRange != filter.Value)),
//                        "any" => query.Where(x => x.VideoStreams.Any() && x.VideoStreams.Any(y => y.VideoRange == filter.Value)),
//                        _ => query
//                    });
//                case "Height":
//                    var heightValues = FormatInputValue(filter.Value);
//                    return (filter.Operation switch
//                    {
//                        "==" => query.Where(x => x.VideoStreams.All(y => y.Height == heightValues[0])),
//                        "<" => query.Where(x => x.VideoStreams.All(y => y.Height < heightValues[0])),
//                        ">" => query.Where(x => x.VideoStreams.All(y => y.Height > heightValues[0])),
//                        "null" => query.Where(x => x.VideoStreams.All(y => y.Height == null)),
//                        "between" => query.Where(x =>
//                            x.VideoStreams.All(y => y.Height > heightValues[0]) &&
//                            x.VideoStreams.All(y => y.Height < heightValues[1])),
//                        _ => query
//                    });
//                case "Width":
//                    var heightWidths = FormatInputValue(filter.Value);
//                    return (filter.Operation switch
//                    {
//                        "==" => query.Where(x => x.VideoStreams.All(y => y.Width == heightWidths[0])),
//                        "<" => query.Where(x => x.VideoStreams.All(y => y.Width < heightWidths[0])),
//                        ">" => query.Where(x => x.VideoStreams.All(y => y.Width > heightWidths[0])),
//                        "null" => query.Where(x => x.VideoStreams.All(y => y.Width == null)),
//                        "between" => query.Where(x =>
//                            x.VideoStreams.All(y => y.Width > heightWidths[0]) &&
//                            x.VideoStreams.All(y => y.Width < heightWidths[1])),
//                        _ => query
//                    });
//                case "AverageFrameRate":
//                    var heightFps = FormatInputValue(filter.Value);
//                    return (filter.Operation switch
//                    {
//                        "==" => query.Where(x => x.VideoStreams.All(y => y.AverageFrameRate == heightFps[0])),
//                        "<" => query.Where(x => x.VideoStreams.All(y => y.AverageFrameRate < heightFps[0])),
//                        ">" => query.Where(x => x.VideoStreams.All(y => y.AverageFrameRate > heightFps[0])),
//                        "null" => query.Where(x => x.VideoStreams.All(y => y.AverageFrameRate == null)),
//                        "between" => query.Where(x =>
//                            x.VideoStreams.All(y => y.AverageFrameRate > heightFps[0]) &&
//                            x.VideoStreams.All(y => y.AverageFrameRate < heightFps[1])),
//                        _ => query
//                    });
//                default:
//                    return ApplyFilter(query, filter);
//            }
//        }
//        #endregion
//    }
//}
