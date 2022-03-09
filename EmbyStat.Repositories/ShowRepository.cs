namespace EmbyStat.Repositories
{
    //public class ShowRepository : MediaRepository<Show>, IShowRepository
    //{
    //    public ShowRepository(IDbContext context) : base(context)
    //    {

    //    }

    //    public Show GetShowByIdWithEpisodes(string showId)
    //    {
    //        return GetShowByIdWithEpisodes(showId, false);
    //    }

    //    public Show GetShowByIdWithEpisodes(string showId, bool includeEpisodes)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var query = database.GetCollection<Show>();

    //        if (includeEpisodes)
    //        {
    //            query = query.Include(x => x.Episodes);
    //        }

    //        return query.FindById(showId);
    //    }

    //    public void RemoveShowsThatAreNotUpdated(DateTimeUtc startTime)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var episodeCollection = database.GetCollection<Episode>();
    //        var seasonCollection = database.GetCollection<Season>();
    //        var showCollection = database.GetCollection<Show>();

    //        var shows = showCollection.Find(x => x.LastUpdated < startTime).ToList();

    //        episodeCollection.DeleteMany(x => shows.Select(y => y.Id).Any(y => y == x.ShowId));
    //        seasonCollection.DeleteMany(x => shows.Select(y => y.Id).Any(y => y == x.ParentId));
    //        showCollection.DeleteMany(x => shows.Select(y => y.Id).Any(y => y == x.Id));
    //    }

    //    public void AddEpisode(Episode episode)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var collection = database.GetCollection<Episode>();
    //        collection.Insert(episode);
    //    }

    //    public void RemoveShows()
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var episodeCollection = database.GetCollection<Episode>();
    //        var seasonCollection = database.GetCollection<Season>();
    //        var showCollection = database.GetCollection<Show>();

    //        episodeCollection.DeleteMany("1=1");
    //        seasonCollection.DeleteMany("1=1");
    //        showCollection.DeleteMany("1=1");
    //    }

    //    public Dictionary<Show, int> GetShowsWithMostEpisodes(IReadOnlyList<string> libraryIds, int count)
    //    {
    //        using var database = Context.CreateDatabaseContext();

    //        var query = database.GetCollection<Show>()
    //            .Query()
    //            .Include(x => x.Seasons)
    //            .Include(x => x.Episodes)
    //            .FilterOnLibrary(libraryIds)
    //            .Select(x => new { Show = x, EpisodeCount = x.GetEpisodeCount(false, LocationType.Disk) }).ToList()
    //            .OrderByDescending(x => x.EpisodeCount)
    //            .Take(count);

    //        return query.ToDictionary(x => x.Show, x => x.EpisodeCount);
    //    }

    //    public IEnumerable<Show> GetShowPage(int skip, int take, string sort, Filter[] filters, List<string> libraryIds)
    //    {

    //        using var database = Context.CreateDatabaseContext();
    //        var query = database.GetCollection<Show>()
    //            .Query()
    //            .Include(x => x.Episodes)
    //            .FilterOnLibrary(libraryIds);

    //        query = filters.Aggregate(query, ApplyShowFilters);

    //        if (!string.IsNullOrWhiteSpace(sort))
    //        {
    //            var jObj = JsonConvert.DeserializeObject<JArray>(sort);
    //            var selector = jObj[0]["selector"].Value<string>().FirstCharToUpper();
    //            var desc = jObj[0]["desc"].Value<bool>();

    //            query = desc
    //                ? query.OrderByDescending(x => typeof(Show).GetProperty(selector).GetValue(x, null))
    //                : query.OrderBy(x => typeof(Show).GetProperty(selector).GetValue(x, null));
    //        }

    //        return query
    //            .Skip(skip)
    //            .Limit(take)
    //            .ToEnumerable();
    //    }

    //    private ILiteQueryable<Show> ApplyShowFilters(ILiteQueryable<Show> query, Filter filter)
    //    {
    //        return ApplyFilter(query, filter);
    //    }

    //    public void UpsertShows(IEnumerable<Show> shows)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var collection = database.GetCollection<Show>();
    //        collection.Upsert(shows);
    //    }

    //    public void UpsertShow(Show show)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var collection = database.GetCollection<Show>();
    //        collection.Upsert(show);
    //    }

    //    public void InsertSeasons(IEnumerable<Season> seasons)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var collection = database.GetCollection<Season>();
    //        collection.Upsert(seasons);
    //    }

    //    public void InsertEpisodes(IEnumerable<Episode> episodes)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var collection = database.GetCollection<Episode>();
    //        collection.Upsert(episodes);
    //    }

    //    public IEnumerable<Show> GetAllShowsWithEpisodes(IReadOnlyList<string> libraryIds, bool includeSeasons, bool includeEpisodes)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var query = database.GetCollection<Show>()
    //            .Query()
    //            .FilterOnLibrary(libraryIds);

    //        if (includeSeasons)
    //        {
    //            query = query.Include(x => x.Seasons);
    //        }

    //        if (includeEpisodes)
    //        {
    //            query = query.Include(x => x.Episodes);
    //        }

    //        return query.ToEnumerable();
    //    }

    //    public Season GetSeasonById(string id)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        var collection = database.GetCollection<Season>();
    //        return collection.FindById(id);
    //    }

    //    public IEnumerable<Episode> GetAllEpisodesForShow(string showId)
    //    {
    //        using var database = Context.CreateDatabaseContext();
    //        return database.GetCollection<Episode>()
    //            .Query()
    //            .Where(x => x.ShowId == showId)
    //            .OrderBy(x => x.IndexNumber)
    //            .ToEnumerable();
    //    }
    //}
}
