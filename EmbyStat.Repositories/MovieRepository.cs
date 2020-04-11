using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Helpers;
using EmbyStat.Repositories.Interfaces;
using MoreLinq;

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
                using (var database = Context.CreateDatabaseContext())
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
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingLibrarySet(collection, libraryIds).OrderBy(x => x.SortName).ToList();
                }
            });
        }

        public List<Movie> GetAllWithImdbId(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.IMDB != null)
                        .OrderBy(x => x.SortName)
                        .ToList();
                }
            });
        }

        public Movie GetMovieById(string id)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return collection.FindById(id);
                }
            });
        }

        public int GetGenreCount(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    var genres = GetWorkingLibrarySet(collection, libraryIds)
                        .Select(x => x.Genres);

                    return genres.SelectMany(x => x)
                        .Distinct()
                        .Count();
                }
            });
        }

        public long GetTotalRuntime(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Select(x => x.RunTimeTicks)
                        .Sum(x => x ?? 0);
                }
            });
        }

        public double GetTotalDiskSize(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Select(x => x.MediaSources.FirstOrDefault())
                        .Sum(x => x?.SizeInMb ?? 0);
                }
            });
        }

        public Movie GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.RunTimeTicks != null && x.RunTimeTicks > toShortMovieTicks)
                        .OrderBy(x => x.RunTimeTicks)
                        .FirstOrDefault();
                }
            });
        }

        public Movie GetLongestMovie(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.RunTimeTicks != null)
                        .OrderByDescending(x => x.RunTimeTicks)
                        .FirstOrDefault();
                }
            });
        }
        
        #region People

        public int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();

                    return GetWorkingLibrarySet(collection, libraryIds)
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
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();

                    return GetWorkingLibrarySet<Movie>(collection, libraryIds)
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
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.RunTimeTicks < TimeSpan.FromMinutes(toShortMovieMinutes).Ticks)
                        .OrderBy(x => x.SortName)
                        .ToList();
                }
            });
        }

        public List<Movie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.IMDB == null)
                        .OrderBy(x => x.SortName)
                        .ToList();
                }
            });
        }

        public List<Movie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.Primary == null)
                        .OrderBy(x => x.SortName)
                        .ToList();
                }
            });
        }

        #endregion

        public void RemoveMovies()
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Movie>();
                    collection.DeleteMany("1=1");
                }
            });
        }
    }
}
