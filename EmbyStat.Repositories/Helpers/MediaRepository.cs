using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Repositories.Interfaces.Helpers;
using LiteDB;

namespace EmbyStat.Repositories.Helpers
{
    public abstract class MediaRepository<T> : BaseRepository, IMediaRepository<T> where T : Extra
    {
        protected MediaRepository(IDbContext context) : base(context)
        {
            
        }

        public IEnumerable<T> GetNewestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.PremiereDate != null)
                        .OrderByDescending(x => x.PremiereDate)
                        .Take(count);
                }
            });
        }

        public IEnumerable<T> GetOldestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.PremiereDate != null)
                        .OrderBy(x => x.PremiereDate)
                        .Take(count);
                }
            });
        }

        public IEnumerable<T> GetHighestRatedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.CommunityRating != null)
                        .OrderByDescending(x => x.CommunityRating)
                        .Take(count);
                }
            });
        }

        public IEnumerable<T> GetLowestRatedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();
                    return GetWorkingLibrarySet(collection, libraryIds)
                        .Where(x => x.CommunityRating != null)
                        .OrderBy(x => x.CommunityRating)
                        .Take(count);
                }
            });
        }

        public IEnumerable<T> GetLatestAddedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();

                    return GetWorkingLibrarySet(collection, libraryIds)
                        .OrderByDescending(x => x.DateCreated)
                        .Take(count);
                }
            });
        }

        public int GetMediaCount(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();
                    return libraryIds.Any()
                        ? collection.Count(Query.In("CollectionId", libraryIds.ConvertToBsonArray()))
                        : collection.Count();
                }
            });
        }

        public bool Any()
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();
                    return collection.Exists(Query.All());
                }
            });
        }

        public int GetMediaCountForPerson(string personId)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();
                    return collection.FindAll().Count(x => x.People.Any(y => personId == y.Id));
                }
            });
        }
    }
}
