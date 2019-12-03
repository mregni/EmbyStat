using System;
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

        public Show GetShowById(int showId)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Show>();

                    return collection.FindById(showId);
                }
            });
        }

        public void RemoveShowsThatAreNotUpdated(DateTime startTime)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var episodeCollection = database.GetCollection<Episode>();
                    var seasonCollection = database.GetCollection<Season>();
                    var showCollection = database.GetCollection<Show>();

                    var shows = showCollection.Find(x => x.LastUpdated < startTime).ToList();

                    episodeCollection.Delete(Query.In("ShowId", shows.Select(x => x.Id).ConvertToBsonArray()));
                    seasonCollection.Delete(Query.In("ParentId", shows.Select(x => x.Id.ToString()).ConvertToBsonArray()));
                    showCollection.Delete(Query.In("_id", shows.Select(x => x.Id).ConvertToBsonArray()));
                }
            });
        }

        public void AddSeason(Season season)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Season>();
                    collection.Upsert(season);
                }
            });
        }

        public void AddEpisode(Episode episode)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Episode>();
                    collection.Upsert(episode);
                }
            });
        }

        public void UpsertShow(Show show)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var episodeCollection = database.GetCollection<Episode>();
                    var seasonCollection = database.GetCollection<Season>();
                    var showCollection = database.GetCollection<Show>();

                    episodeCollection.Delete(x => x.ShowId == show.Id);
                    seasonCollection.Delete(x => x.ParentId == show.Id.ToString());
                    showCollection.Delete(x => x.Id == show.Id);
                    
                    episodeCollection.Upsert(show.Episodes);
                    seasonCollection.Upsert(show.Seasons);
                    showCollection.Upsert(show);
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
