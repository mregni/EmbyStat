using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using MediaBrowser.Model.Extensions;
using NLog;
using Logger = NLog.Logger;

namespace EmbyStat.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly LiteCollection<Show> _showCollection;
        private readonly LiteCollection<Season> _seasonCollection;
        private readonly LiteCollection<Episode> _episodeCollection;
        private readonly Logger _logger;

        public ShowRepository(IDbContext context)
        {
            _showCollection = context.GetContext().GetCollection<Show>();
            _seasonCollection = context.GetContext().GetCollection<Season>();
            _episodeCollection = context.GetContext().GetCollection<Episode>();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void RemoveShows()
        {
            _showCollection.Delete(Query.All());
        }

        public void UpsertShows(IEnumerable<Show> list)
        {
            _showCollection.Upsert(list);
        }

        public void UpsertSeasons(IEnumerable<Season> seasons)
        {
            _seasonCollection.Upsert(seasons);
        }

        public void UpsertEpisodes(IEnumerable<Episode> episodes)
        {
            _episodeCollection.Upsert(episodes);
        }

        public void UpdateShow(Show show)
        {
            _showCollection.Update(show);
        }

        public IEnumerable<Show> GetAllShows(IEnumerable<string> collectionIds)
        {
            return collectionIds.Any() ?
                _showCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))) :
                _showCollection.FindAll();
        }

        public Season GetSeasonById(string id)
        {
            return _showCollection
                .Find(Query.EQ("Seasons[*].Id", id), 0, 1)
                .SingleOrDefault()
                ?.Seasons
                .SingleOrDefault(x => x.Id == id);
        }

        public IEnumerable<Show> GetAllShowsWithTvdbId()
        {
            return _showCollection.Find(x => !string.IsNullOrWhiteSpace(x.TVDB));
        }

        public IEnumerable<Season> GetAllSeasonsForShow(string showId)
        {
            return _showCollection.FindById(showId).Seasons;
        }

        public IEnumerable<Episode> GetAllEpisodesForShow(string showId)
        {
            return _showCollection.FindById(showId).Episodes;
        }

        public IEnumerable<Episode> GetAllEpisodesForShows(IEnumerable<string> showIds)
        {
            return _showCollection.Find(x => showIds.Any(y => y == x.Id)).SelectMany(x => x.Episodes);
        }

        public int CountShows(IEnumerable<string> collectionIds)
        {
            if (collectionIds.Any())
            {
                return _showCollection.Count(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y)));
            }

            return _showCollection.Count(Query.All());
        }

        public int CountEpisodes(IEnumerable<string> collectionIds)
        {
            if (collectionIds.Any())
            {
                return _showCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))).Sum(x => x.Episodes.Count());
            }

            return _showCollection.FindAll().Sum(x => x.Episodes.Count());
        }

        public int CountEpisodes(string showId)
        {
            return _showCollection.Find(x => x.Id == showId).Sum(x => x.Episodes.Count());
        }

        public long GetPlayLength(IEnumerable<string> collectionIds)
        {
            if (collectionIds.Any())
            {
                return _showCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))).Sum(x => x.RunTimeTicks ?? 0);
            }

            return _showCollection.FindAll().Sum(x => x.RunTimeTicks ?? 0);
        }

        public int GetTotalPeopleByType(IEnumerable<string> collectionIds, string type)
        {
            var shows = collectionIds.Any() ?
                _showCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))) :
                _showCollection.FindAll();

            return shows
                .SelectMany(x => x.People)
                .DistinctBy(x => x.Id)
                .Count(x => x.Type == type);
        }

        public string GetMostFeaturedPerson(IEnumerable<string> collectionIds, string type)
        {
            var shows = collectionIds.Any() ? 
                _showCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))) : 
                _showCollection.FindAll();

            return shows
                .SelectMany(x => x.People)
                .Where(x => x.Type == type)
                .GroupBy(x => x.Id, x => x.Name, (id, name) => new { Key = id, Count = name.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Key)
                .FirstOrDefault();
        }

        public IEnumerable<string> GetGenres(IEnumerable<string> collectionIds)
        {
            var shows = collectionIds.Any() ?
                _showCollection.Find(x => collectionIds.Any(y => x.CollectionIds.Any(z => z == y))) :
                _showCollection.FindAll();

            return shows
                .SelectMany(x => x.GenresIds)
                .Distinct();
        }

        public int GetEpisodeCountForShow(string showId, bool includeSpecials = false)
        {
            var show = _showCollection.FindById(showId);

            if (!includeSpecials)
            {
                return show.Episodes.Count(x => x.IndexNumber != 0);
            }

            return show.Episodes.Count();
        }

        public int GetSeasonCountForShow(string showId, bool includeSpecials = false)
        {
            var show = _showCollection.FindById(showId);

            if (!includeSpecials)
            {
                return show.Seasons.Count(x => x.IndexNumber != 0);
            }
            
            return show.Seasons.Count();
        }

        public bool Any()
        {
            return _showCollection.Exists(Query.All());
        }

        public Episode GetEpisodeById(string id)
        {
            var show = _showCollection.Find(Query.EQ("Episodes[*].Id", id));
            return show.Single().Episodes.Single(x => x.Id == id);
        }
    }
}
