using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using MediaBrowser.Model.Extensions;
using NLog;
using Logger = NLog.Logger;

namespace EmbyStat.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IDbContext _context;

        public MovieRepository(IDbContext context)
        {
            _context = context;
        }

        public void UpsertRange(IEnumerable<Movie> movies)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                collection.Upsert(movies);
            }
        }

        public List<Movie> GetAll(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds).ToList();
            }
        }


        public bool Any()
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return collection.Exists(Query.All());
            }
        }

        public int GetMovieCountForPerson(string personId)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return collection.Count(Query.EQ("People[*]._id", personId));
            }
        }

        public Movie GetMovieById(string id)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return collection.FindById(id);
            }
        }

        public int GetMovieCount(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return libraryIds.Any()
                    ? collection.Count(Query.In("CollectionId", libraryIds.ConvertToBsonArray()))
                    : collection.Count();
            }
        }

        public int GetGenreCount(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds)
                    .Select(x => x.Genres)
                    .ToList()
                    .SelectMany(x => x)
                    .Distinct()
                    .Count();
            }
        }

        public Movie GetHighestRatedMovie(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds)
                    .Where(x => x.CommunityRating != null)
                    .OrderByDescending(x => x.CommunityRating)
                    .FirstOrDefault();
            }
        }

        public long GetTotalRuntime(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds)
                    .Select(x => x.RunTimeTicks)
                    .ToList()
                    .Sum(x => x ?? 0);
            }
        }

        public Movie GetLowestRatedMovie(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds)
                    .Where(x => x.CommunityRating != null)
                    .OrderBy(x => x.CommunityRating)
                    .FirstOrDefault();
            }
        }

        public Movie GetOldestPremiered(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds)
                    .Where(x => x.PremiereDate != null)
                    .OrderBy(x => x.PremiereDate)
                    .FirstOrDefault();
            }
        }

        public Movie GetNewestPremiered(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds)
                    .Where(x => x.PremiereDate != null)
                    .OrderByDescending(x => x.PremiereDate)
                    .FirstOrDefault();
            }
        }

        public Movie GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks)
        {

            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds)
                    .Where(x => x.RunTimeTicks != null && x.RunTimeTicks >= toShortMovieTicks)
                    .OrderBy(x => x.RunTimeTicks)
                    .FirstOrDefault();
            }
        }

        public Movie GetLongestMovie(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds)
                    .Where(x => x.RunTimeTicks != null)
                    .OrderByDescending(x => x.RunTimeTicks)
                    .FirstOrDefault();
            }
        }

        public Movie GetLatestAdded(IReadOnlyList<string> libraryIds)
            {
                using (var database = _context.GetContext())
                {
                    var collection = database.GetCollection<Movie>();
                    return GetWorkingMovieSet(collection, libraryIds)
                        .Where(x => x.DateCreated != null)
                        .OrderByDescending(x => x.DateCreated)
                        .FirstOrDefault();
                }
            }

        public double GetTotalDiskSize(IReadOnlyList<string> libraryIds)
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                return GetWorkingMovieSet(collection, libraryIds)
                    .Select(x => x.MediaSources.FirstOrDefault())
                    .ToList()
                    .Sum(x => x.SizeInMb);
            }
        }

        private ILiteQueryable<Movie> GetWorkingMovieSet(LiteCollection<Movie> collection, IReadOnlyList<string> libraryIds)
        {
            return libraryIds.Any()
                ? collection.Query().Where(x => libraryIds.Any(y => y == x.CollectionId))
                : collection.Query();
        }

        public void RemoveMovies()
        {
            using (var database = _context.GetContext())
            {
                var collection = database.GetCollection<Movie>();
                collection.DeleteMany(x => true);
            }
        }
    }
}
