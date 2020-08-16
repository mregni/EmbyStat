using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Helpers;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class ShowRepository : MediaRepository<Show>, IShowRepository
    {
        public ShowRepository(IDbContext context) : base(context)
        {

        }

        public Show GetShowById(string showId)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Show>();

                return collection.FindById(showId);
            });
        }

        public void RemoveShowsThatAreNotUpdated(DateTime startTime)
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var episodeCollection = database.GetCollection<Episode>();
                var seasonCollection = database.GetCollection<Season>();
                var showCollection = database.GetCollection<Show>();

                var shows = showCollection.Find(x => x.LastUpdated < startTime).ToList();

                episodeCollection.DeleteMany(x => shows.Select(y => y.Id).Any(y => y == x.ShowId));
                seasonCollection.DeleteMany(x => shows.Select(y => y.Id).Any(y => y == x.ParentId));
                showCollection.DeleteMany(x => shows.Select(y => y.Id).Any(y => y == x.Id));
            });
        }

        public void AddSeason(Season season)
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Season>();
                collection.Insert(season);
            });
        }

        public void AddEpisode(Episode episode)
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Episode>();
                collection.Insert(episode);
            });
        }

        public void RemoveShows()
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var episodeCollection = database.GetCollection<Episode>();
                var seasonCollection = database.GetCollection<Season>();
                var showCollection = database.GetCollection<Show>();

                episodeCollection.DeleteMany("1=1");
                seasonCollection.DeleteMany("1=1");
                showCollection.DeleteMany("1=1");
            });
        }

        public Dictionary<Show, int> GetShowsWithMostEpisodes(IReadOnlyList<string> libraryIds, int count)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var query = database.GetCollection<Show>()
                    .Include(x => x.Seasons)
                    .Include(x => x.Episodes);

                var list = libraryIds.Any() ? query.Find(x => libraryIds.Any(y => y == x.CollectionId)) : query.FindAll();
                return list
                    .Select(x => new {Show = x, EpisodeCount = x.GetNonSpecialEpisodeCount(false)})
                    .OrderByDescending(x => x.EpisodeCount)
                    .Take(count)
                    .ToDictionary(x => x.Show, x => x.EpisodeCount);
            });
        }

        public void InsertShow(Show show)
        {
            ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var episodeCollection = database.GetCollection<Episode>();
                var seasonCollection = database.GetCollection<Season>();
                var showCollection = database.GetCollection<Show>();

                episodeCollection.DeleteMany(x => x.ShowId == show.Id);
                seasonCollection.DeleteMany(x =>  x.ParentId == show.Id);
                showCollection.DeleteMany(x => x.Id == show.Id);
                database.Commit();

                episodeCollection.Upsert(show.Episodes);
                seasonCollection.Upsert(show.Seasons);
                showCollection.Upsert(show);
            });
        }

        public List<Show> GetAllShows(IReadOnlyList<string> libraryIds, bool includeSeasons, bool includeEpisodes)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
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
                    return collection.Find(x => libraryIds.Any(y => y == x.CollectionId)).ToList();
                }

                return collection.FindAll().ToList();
            });
        }

        public Season GetSeasonById(string id)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Season>();
                return collection.FindById(id);
            });
        }

        public List<Episode> GetAllEpisodesForShow(string showId)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Episode>();
                return collection.Find(x => x.ShowId == showId).OrderBy(x => x.IndexNumber).ToList();
            });
        }

        public Episode GetEpisodeById(string showId, string id)
        {
            return ExecuteQuery(() =>
            {
                using var database = Context.CreateDatabaseContext();
                var collection = database.GetCollection<Episode>();
                return collection.Find(x => x.Id == id && x.ShowId == showId).SingleOrDefault();
            });
        }
    }
}
