using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Extensions;

namespace EmbyStat.Repositories
{
    public class MovieRepository : BaseRepository, IMovieRepository
    {
        private readonly IDbContext _context;

        public MovieRepository(IDbContext context)
        {
            _context = context;
        }

        public void UpsertRange(IEnumerable<Movie> movies)
        {
            ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    collection.Upsert(movies);
                }
            });
        }

        public List<Movie> GetAll(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingMovieSet(collection, libraryIds).ToList();
                }
            });
        }

        public List<Movie> GetAllWithImdbId(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    if (libraryIds.Any())
                    {
                        return collection
                            .Find(x => x.IMDB != null && libraryIds.Any(y => x.CollectionId == y))
                            .OrderBy(x => x.SortName)
                            .ToList();
                    }

                    return collection
                        .Find(x => x.IMDB != null)
                        .OrderBy(x => x.SortName)
                        .ToList();
                }
            });
        }


        public bool Any()
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return collection.Exists(Query.All());
                }
            });
        }

        public int GetMovieCountForPerson(string personId)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return collection.Count(x => x.People.Any(y => y.Id == personId));
                }
            });
        }

        public Movie GetMovieById(string id)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return collection.FindById(id);
                }
            });
        }

        public int GetMovieCount(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return libraryIds.Any()
                        ? collection.Count(Query.In("CollectionId", libraryIds.ConvertToBsonArray()))
                        : collection.Count();
                }
            });
        }

        public int GetGenreCount(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingMovieSet(collection, libraryIds)
                        .Select(x => x.Genres)
                        .ToList()
                        .SelectMany(x => x)
                        .Distinct()
                        .Count();
                }
            });
        }

        public long GetTotalRuntime(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingMovieSet(collection, libraryIds)
                        .Select(x => x.RunTimeTicks)
                        .ToList()
                        .Sum(x => x ?? 0);
                }
            });
        }

        public double GetTotalDiskSize(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingMovieSet(collection, libraryIds)
                        .Select(x => x.MediaSources.FirstOrDefault())
                        .ToList()
                        .Sum(x => x.SizeInMb);
                }
            });
        }

        public Movie GetHighestRatedMovie(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    if (libraryIds.Any())
                    {
                       return collection
                            .Find(x => x.CommunityRating != null && libraryIds.Any(y => x.CollectionId == y))
                            .OrderByDescending(x => x.CommunityRating)
                            .FirstOrDefault();
                    }

                    return collection
                        .Find(x => x.CommunityRating != null)
                        .OrderByDescending(x => x.CommunityRating)
                        .FirstOrDefault();
                }
            });
        }

        public Movie GetLowestRatedMovie(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    if (libraryIds.Any())
                    {
                        return collection
                            .Find(x => x.CommunityRating != null && libraryIds.Any(y => x.CollectionId == y))
                            .OrderBy(x => x.CommunityRating)
                            .FirstOrDefault();
                    }

                    return collection
                        .Find(x => x.CommunityRating != null)
                        .OrderBy(x => x.CommunityRating)
                        .FirstOrDefault();
                }
            });
        }

        public Movie GetOldestPremiered(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();

                    if (libraryIds.Any())
                    {
                        return collection
                            .Find(x => x.PremiereDate != null && libraryIds.Any(y => x.CollectionId == y))
                            .OrderBy(x => x.PremiereDate)
                            .FirstOrDefault();
                    }

                    return collection
                        .Find(x => x.PremiereDate != null)
                        .OrderBy(x => x.PremiereDate)
                        .FirstOrDefault();
                }
            });
        }

        public Movie GetNewestPremiered(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();

                    if (libraryIds.Any())
                    {
                        return collection
                            .Find(x => x.PremiereDate != null && libraryIds.Any(y => x.CollectionId == y))
                            .OrderByDescending(x => x.PremiereDate)
                            .FirstOrDefault();
                    }

                    return collection
                        .Find(x => x.PremiereDate != null)
                        .OrderByDescending(x => x.PremiereDate)
                        .FirstOrDefault();
                }
            });
        }

        public Movie GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    if (libraryIds.Any())
                    {
                        return collection
                            .Find(x => x.RunTimeTicks != null && libraryIds.Any(y => x.CollectionId == y))
                            .OrderBy(x => x.RunTimeTicks)
                            .FirstOrDefault();
                    }

                    return collection
                        .Find(x => x.RunTimeTicks != null)
                        .OrderBy(x => x.RunTimeTicks)
                        .FirstOrDefault();
                }
            });
        }

        public Movie GetLongestMovie(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    if (libraryIds.Any())
                    {
                        return collection
                            .Find(x => x.RunTimeTicks != null && libraryIds.Any(y => x.CollectionId == y))
                            .OrderByDescending(x => x.RunTimeTicks)
                            .FirstOrDefault();
                    }

                    return collection
                        .Find(x => x.RunTimeTicks != null)
                        .OrderByDescending(x => x.RunTimeTicks)
                        .FirstOrDefault();
                }
            });
        }

        public Movie GetLatestAdded(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();

                    return GetWorkingMovieSet(collection, libraryIds)
                        .OrderBy(x => x.DateCreated)
                        .FirstOrDefault();
                }
            });
        }

        #region People

        public int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();

                    return GetWorkingMovieSet(collection, libraryIds)
                        .SelectMany(x => x.People)
                        .DistinctBy(x => x.Id)
                        .Count(x => x.Type == type);
                }
            });
        }

        public string GetMostFeaturedPerson(IReadOnlyList<string> libraryIds, PersonType type)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();

                    return GetWorkingMovieSet(collection, libraryIds)
                        .SelectMany(x => x.People)
                        .Where(x => x.Type == type)
                        .GroupBy(x => x.Name, (name, people) => new {Name = name, Count = people.Count()})
                        .OrderByDescending(x => x.Count)
                        .Select(x => x.Name)
                        .FirstOrDefault();
                }
            });
        }

        #endregion

        #region Suspicious

        public List<Movie> GetToShortMovieList(IReadOnlyList<string> libraryIds, int toShortMovieMinutes)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    if (libraryIds.Any())
                    {
                        return collection
                            .Find(x => x.RunTimeTicks < new TimeSpan(toShortMovieMinutes).Ticks && libraryIds.Any(y => x.CollectionId == y))
                            .OrderBy(x => x.SortName)
                            .ToList();
                    }

                    return collection
                        .Find(x => x.RunTimeTicks < new TimeSpan(toShortMovieMinutes).Ticks)
                        .OrderBy(x => x.SortName)
                        .ToList();
                }
            });
        }

        public List<Movie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    if (libraryIds.Any())
                    {
                        return collection
                            .Find(x => x.IMDB == null && libraryIds.Any(y => x.CollectionId == y))
                            .OrderBy(x => x.SortName)
                            .ToList();
                    }

                    return collection
                        .Find(x => x.IMDB == null)
                        .OrderBy(x => x.SortName)
                        .ToList();
                }
            });
        }

        public List<Movie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    if (libraryIds.Any())
                    {
                        return collection
                            .Find(x => x.Primary == null && libraryIds.Any(y => x.CollectionId == y))
                            .OrderBy(x => x.SortName)
                            .ToList();
                    }

                    return collection
                        .Find(x => x.Primary == null)
                        .OrderBy(x => x.SortName)
                        .ToList();
                }
            });
        }

        #endregion

        private static IEnumerable<Movie> GetWorkingMovieSet(LiteCollection<Movie> collection, IReadOnlyList<string> libraryIds)
        {
            return libraryIds.Any() 
                ? collection.Find(Query.In("CollectionId", libraryIds.ConvertToBsonArray())) 
                : collection.FindAll();

            ;
        }

        public void RemoveMovies()
        {
            ExecuteQuery(() =>
            {
                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    collection.Delete(Query.All());
                }
            });
        }
    }
}
