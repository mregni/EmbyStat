using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Helpers;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class ShowRepository : MediaRepository<Show>, IShowRepository
    {
        public ShowRepository(IDbContext context) : base(context)
        {

        }

        public void RemoveShows()
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var seasonCollection = database.GetCollection<Season>();
                    var episodeCollection = database.GetCollection<Episode>();
                    var showCollection = database.GetCollection<Show>();

                    seasonCollection.Delete(Query.All());
                    episodeCollection.Delete(Query.All());
                    showCollection.Delete(Query.All());
                }
            });
        }

        public void InsertShow(Show show)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var episodeCollection = database.GetCollection<Episode>();
                    episodeCollection.InsertBulk(show.Episodes);

                    var seasonCollection = database.GetCollection<Season>();
                    seasonCollection.InsertBulk(show.Seasons);

                    var collection = database.GetCollection<Show>();
                    collection.Insert(show);
                }
            });
        }

        public void UpdateShow(Show show)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Show>();
                    collection.Update(show);
                }
            });
        }

        public List<Show> GetAllShows(IReadOnlyList<string> libraryIds, bool includeSeasons, bool includeEpisodes)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Show>();
                    if (includeSeasons)
                    {
                        collection = collection.Include(x => x.Seasons);
                    }

                    if (includeEpisodes)
                    {
                        collection = collection.Include(x => x.Episodes);
                    }

                    if (libraryIds.Any())
                    {
                        return collection.Find(Query.In("CollectionId", libraryIds.ConvertToBsonArray())).ToList();
                    }

                    return collection.FindAll().ToList();
                }
            });
        }

        public Season GetSeasonById(int id)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Season>();
                    return collection.FindById(id);
                }
            });
        }

        public List<Show> GetAllShowsWithTvdbId()
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Show>();
                    return collection
                        .Include(x => x.Episodes)
                        .Include(x => x.Seasons)
                        .Find(x => !string.IsNullOrWhiteSpace(x.TVDB))
                        .ToList();
                }
            });
        }

        public List<Episode> GetAllEpisodesForShow(int showId)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Episode>();
                    return collection.Find(Query.EQ("ShowId", showId)).ToList();
                }
            });
        }

        public Episode GetEpisodeById(int id)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Episode>();
                    return collection.FindById(id);
                }
            });
        }
    }
}
