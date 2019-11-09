using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Repositories.Interfaces.Helpers;
using LiteDB;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Repositories.Helpers
{
    public abstract class MediaRepository<T> : BaseRepository, IMediaRepository<T> where T : Extra
    {
        protected MediaRepository(IDbContext context) : base(context)
        {
            
        }

        public T GetNewestPremieredMedia(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();

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

        public T GetOldestPremieredMedia(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();

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

        public T GetHighestRatedMedia(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();
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

        public T GetLowestRatedMedia(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();
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

        public T GetLatestAddedMedia(IReadOnlyList<string> libraryIds)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<T>();

                    return GetWorkingLibrarySet(collection, libraryIds)
                        .OrderByDescending(x => x.DateCreated)
                        .FirstOrDefault();
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
                    return collection.FindAll().Count(x => x.People.Any(y => y.Id == personId));
                }
            });
        }
    }
}
