using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.Shows.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using MoreLinq.Extensions;

namespace EmbyStat.Core.Shows;

public class ShowRepository : IShowRepository
{
    private readonly EsDbContext _context;
    private readonly ISqliteBootstrap _sqliteBootstrap;
    private readonly ILogger<ShowRepository> _logger;

    public ShowRepository(EsDbContext context, ISqliteBootstrap sqliteBootstrap, ILogger<ShowRepository> logger)
    {
        _context = context;
        _sqliteBootstrap = sqliteBootstrap;
        _logger = logger;
    }

    public async Task<IEnumerable<Media>> GetNewestPremieredMedia(int count)
    {
        var query = MediaExtensions.GenerateGetPremieredListQuery(count, "DESC", Constants.Tables.Shows);
        return await ExecuteListQueryWithLibraryIds<Show>(query);
    }

    public async Task<IEnumerable<Media>> GetOldestPremieredMedia(int count)
    {
        var query = MediaExtensions.GenerateGetPremieredListQuery(count, "ASC", Constants.Tables.Shows);
        return await ExecuteListQueryWithLibraryIds<Show>(query);
    }

    public async Task<IEnumerable<Extra>> GetHighestRatedMedia(int count)
    {
        var query = MediaExtensions.GenerateGetCommunityRatingListQuery(count, "DESC", Constants.Tables.Shows);
        return await ExecuteListQueryWithLibraryIds<Show>(query);
    }

    public async Task<IEnumerable<Extra>> GetLowestRatedMedia(int count)
    {
        var query = MediaExtensions.GenerateGetCommunityRatingListQuery(count, "ASC", Constants.Tables.Shows);
        return await ExecuteListQueryWithLibraryIds<Show>(query);
    }

    public IEnumerable<Media> GetLatestAddedMedia(int count)
    {
        return _context.Shows.GetLatestAddedMedia(count);
    }

    public async Task<Dictionary<string, int>> GetGenreChartValues()
    {
        var query = $@"SELECT g.Name G, COUNT(*) Count
FROM {Constants.Tables.Shows} AS s
INNER JOIN {Constants.Tables.GenreShow} as gs ON (s.Id = gs.ShowsId)
INNER JOIN {Constants.Tables.Genres} as g ON (g.Id = gs.GenresId)
GROUP BY g.Name
ORDER BY g.Name";
        
        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        return connection.Query(query)
            .ToDictionary(
                row => (string) row.G,
                row => (int) row.Count);
    }

    public IEnumerable<decimal?> GetCommunityRatings()
    {
        return _context.Shows
            .Select(x => x.CommunityRating);
    }

    public IEnumerable<DateTime?> GetPremiereYears()
    {
        return _context.Shows
            .Select(x => x.PremiereDate);
    }

    public async Task<Dictionary<string, int>> GetOfficialRatingChartValues()
    {
        var query = $@"SELECT upper(s.OfficialRating) OfficialRating, COUNT(*) Count
FROM {Constants.Tables.Shows} AS s
WHERE s.OfficialRating IS NOT NULL 
GROUP BY upper(s.OfficialRating)
ORDER BY OfficialRating";
        
        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        return connection.Query(query)
            .ToDictionary(
                row => (string) row.OfficialRating,
                row => (int) row.Count);
    }

    public async Task<Dictionary<string, int>> GetShowStatusCharValues()
    {
        var query = $@"SELECT s.Status Status, COUNT(*) Count
FROM {Constants.Tables.Shows} AS s
WHERE s.Status IS NOT NULL 
GROUP BY s.Status
ORDER BY s.Status";

        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        return connection.Query(query)
            .ToDictionary(
                row => (string) row.Status,
                row => (int) row.Count);
    }

    public async Task<IEnumerable<double>> GetCollectedRateChart()
    {
        var query = $@"SELECT ROUND(CAST(COUNT(*) FILTER(WHERE e.LocationType = 0) AS FLOAT) / COUNT(*), 2) AS pct
FROM {Constants.Tables.Shows} AS s
INNER JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)
INNER JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)
WHERE se.IndexNumber != 0 
GROUP BY s.Id
ORDER BY s.Id";

        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        return connection.Query<double>(query);
    }

    public Task<int> Count()
    {
        return Count(Array.Empty<Filter>());
    }

    public async Task<int> Count(Filter[] filters)
    {
        var query = ShowExtensions.GenerateCountQuery(filters);

        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        var result = await connection.QueryFirstAsync<int>(query);

        return result;
    }

    public IEnumerable<Show> GetShowsWithMostDiskSpaceUsed(int count)
    {
        return _context.Shows
            .OrderByDescending(x => x.SizeInMb)
            .Take(count);
    }

    public IEnumerable<string> GetShowIdsThatFailedExternalSync(string libraryId)
    {
        return _context.Shows
            .Include(x => x.Library)
            .Where(x => !x.ExternalSynced && x.LibraryId == libraryId)
            .Select(x => x.Id);
    }

    public async Task<int> CompleteCollectedCount()
    {
        var query = $@"SELECT COUNT(*) Count 
FROM {Constants.Tables.Shows} AS s 
WHERE NOT EXISTS (SELECT 1 FROM {Constants.Tables.Seasons} AS se INNER JOIN {Constants.Tables.Episodes} AS ep ON (se.Id = ep.SeasonId) 
    WHERE se.ShowId = s.Id AND ep.LocationType = 1) ";

        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        return connection.QueryFirst<int>(query);
    }

    public bool Any()
    {
        return _context.Shows.Any();
    }

    public async Task<int> GetGenreCount()
    {
        var query = $@"SELECT COUNT(DISTINCT g.Name)
FROM {Constants.Tables.Shows} AS s
INNER JOIN {Constants.Tables.GenreShow} AS gs ON (s.Id = gs.ShowsId)
INNER JOIN {Constants.Tables.Genres} AS g On (g.Id = gs.GenresId)
WHERE 1=1 ";

        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        return await connection.QueryFirstAsync<int>(query);
    }

    public int GetPeopleCount(PersonType type)
    {
        return _context.Shows
            .Include(x => x.People)
            .SelectMany(x => x.People)
            .Distinct()
            .Count(x => x.Type == type);
    }

    #region Shows

    public async Task UpsertShows(IEnumerable<Show> shows)
    {
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();

        var showList = shows.ToList();
        await using var deleteTransaction = connection.BeginTransaction();
        const string deleteQuery = "DELETE FROM Shows WHERE Id IN @Ids";
        await connection.ExecuteAsync(deleteQuery, new {Ids = showList.Select(x => x.Id)}, deleteTransaction);
        await deleteTransaction.CommitAsync();

        foreach (var show in showList)
        {
            await using var transaction = connection.BeginTransaction();
            
            var showQuery =
                $@"INSERT OR REPLACE INTO {Constants.Tables.Shows} (Id,DateCreated,Banner,Logo,""Primary"",Thumb,Name,Path,PremiereDate,ProductionYear,SortName,CommunityRating,IMDB,TMDB,TVDB,RunTimeTicks,OfficialRating,CumulativeRunTimeTicks,Status,ExternalSynced,SizeInMb,LibraryId)
VALUES (@Id,@DateCreated,@Banner,@Logo,@Primary,@Thumb,@Name,@Path,@PremiereDate,@ProductionYear,@SortName,@CommunityRating,@IMDB,@TMDB,@TVDB,@RunTimeTicks,@OfficialRating,@CumulativeRunTimeTicks,@Status,@ExternalSynced,@SizeInMb,@LibraryId)";
            var showParams = new DynamicParameters(show);
            showParams.Add("@LibraryId",show.Library.Id);
            
            await connection.ExecuteAsync(showQuery, showParams, transaction);

            if (show.Seasons.AnyNotNull())
            {
                ForEachExtension.ForEach(show.Seasons, x => x.ShowId = show.Id);
                var seasonQuery =
                    $@"INSERT OR REPLACE INTO {Constants.Tables.Seasons} (Id,DateCreated,Banner,Logo,""Primary"",Thumb,Name,Path,PremiereDate,ProductionYear,SortName,IndexNumber,IndexNumberEnd,LocationType,ShowId)
VALUES (@Id,@DateCreated,@Banner,@Logo,@Primary,@Thumb,@Name,@Path,@PremiereDate,@ProductionYear,@SortName,@IndexNumber,@IndexNumberEnd,@LocationType,@ShowId)";

                await connection.ExecuteAsync(seasonQuery, show.Seasons, transaction);
            }

            if (show.Genres.AnyNotNull())
            {
                var genreQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.GenreShow} (GenresId, ShowsId) 
VALUES (@GenreId, @ShowsId)";
                var genreList = show.Genres.Select(x => new {GenreId = x.Id, ShowsId = show.Id});

                await connection.ExecuteAsync(genreQuery, genreList, transaction);
            }

            if (show.People.AnyNotNull())
            {
                var peopleQuery =
                    @$"INSERT OR REPLACE INTO {Constants.Tables.MediaPerson} (Type, ShowId, PersonId)
VALUES (@Type, @ShowId, @PersonId)";
                ForEachExtension.ForEach(show.People, x => x.ShowId = show.Id);

                await connection.ExecuteAsync(peopleQuery, show.People, transaction);
            }

            var episodes = show.Seasons.SelectMany(x => x.Episodes).ToList();
            if (episodes.AnyNotNull())
            {
                var episodeQuery =
                    @$"INSERT OR REPLACE INTO {Constants.Tables.Episodes} (Id,DateCreated,Banner,Logo,""Primary"",Thumb,Name,Path,PremiereDate,ProductionYear,SortName,Container,CommunityRating,IMDB,TMDB,TVDB,RunTimeTicks,OfficialRating,Video3DFormat,DvdEpisodeNumber,DvdSeasonNumber,IndexNumber,IndexNumberEnd,SeasonId,LocationType)
VALUES (@Id,@DateCreated,@Banner,@Logo,@Primary,@Thumb,@Name,@Path,@PremiereDate,@ProductionYear,@SortName,@Container,@CommunityRating,@IMDB,@TMDB,@TVDB,@RunTimeTicks,@OfficialRating,@Video3DFormat,@DvdEpisodeNumber,@DvdSeasonNumber,@IndexNumber,@IndexNumberEnd,@SeasonId,@LocationType)";

                await connection.ExecuteAsync(episodeQuery, episodes, transaction);

                foreach (var episode in episodes)
                {
                    if (episode.MediaSources.AnyNotNull())
                    {
                        var mediaSourceQuery =
                            @$"INSERT OR REPLACE INTO {Constants.Tables.MediaSources} (Id,BitRate,Container,Path,Protocol,RunTimeTicks,SizeInMb,EpisodeId) 
VALUES (@Id, @BitRate,@Container,@Path,@Protocol,@RunTimeTicks,@SizeInMb,@EpisodeId)";
                        ForEachExtension.ForEach(episode.MediaSources, x => x.EpisodeId = episode.Id);

                        await connection.ExecuteAsync(mediaSourceQuery, episode.MediaSources, transaction);
                    }

                    if (episode.VideoStreams.AnyNotNull())
                    {
                        var videoStreamQuery =
                            @$"INSERT OR REPLACE INTO {Constants.Tables.VideoStreams} (Id,AspectRatio,AverageFrameRate,BitRate,Channels,Height,Language,Width,BitDepth,Codec,IsDefault,VideoRange,EpisodeId) 
VALUES (@Id,@AspectRatio,@AverageFrameRate,@BitRate,@Channels,@Height,@Language,@Width,@BitDepth,@Codec,@IsDefault,@VideoRange,@EpisodeId)";
                        ForEachExtension.ForEach(episode.VideoStreams, x => x.EpisodeId = episode.Id);

                        await connection.ExecuteAsync(videoStreamQuery, episode.VideoStreams, transaction);
                    }

                    if (episode.AudioStreams.AnyNotNull())
                    {
                        var audioStreamQuery =
                            @$"INSERT OR REPLACE INTO {Constants.Tables.AudioStreams} (Id,BitRate,ChannelLayout,Channels,Codec,Language,SampleRate,IsDefault,EpisodeId)
VALUES (@Id,@BitRate,@ChannelLayout,@Channels,@Codec,@Language,@SampleRate,@IsDefault,@EpisodeId)";
                        ForEachExtension.ForEach(episode.AudioStreams, x => x.EpisodeId = episode.Id);

                        await connection.ExecuteAsync(audioStreamQuery, episode.AudioStreams, transaction);
                    }

                    if (episode.SubtitleStreams.AnyNotNull())
                    {
                        var subtitleStreamQuery =
                            @$"INSERT OR REPLACE INTO {Constants.Tables.SubtitleStreams} (Id,Codec,DisplayTitle,IsDefault,Language,EpisodeId)
VALUES (@Id,@Codec,@DisplayTitle,@IsDefault,@Language,@EpisodeId)";
                        ForEachExtension.ForEach(episode.SubtitleStreams, x => x.EpisodeId = episode.Id);

                        await connection.ExecuteAsync(subtitleStreamQuery, episode.SubtitleStreams,
                            transaction);
                    }
                }

                await transaction.CommitAsync();
            }
        }
    }

    public async Task<IEnumerable<Show>> GetAllShowsWithEpisodes()
    {
        var query = ShowExtensions.GenerateFullShowQuery();
        
        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();

        var list = await connection.QueryAsync<Show, Season, Episode, Show>(query, (s, se, e) =>
        {
            s.Seasons ??= new List<Season>();
            se.Episodes ??= new List<Episode>();

            se.Episodes.AddIfNotNull(e);
            s.Seasons.AddIfNotNull(se);
            return s;
        });

        return MapShows(list);
    }

    public async Task<Show> GetShowByIdWithEpisodes(string showId)
    {
        var query = ShowExtensions.GenerateFullShowWithGenresQuery();
        query += $" AND s.Id = '{showId}'";

        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();

        _logger.LogDebug("{Query}", query);
        var list = await connection.QueryAsync<Show, Genre, Season, Episode, Show>(query,
            (s, g, se, e) =>
            {
                s.Genres ??= new List<Genre>();
                s.Seasons ??= new List<Season>();
                se.Episodes ??= new List<Episode>();

                s.Genres.AddIfNotNull(g);
                se.Episodes.AddIfNotNull(e);
                s.Seasons.AddIfNotNull(se);
                return s;
            }, new {Id = showId});

        return MapShows(list).FirstOrDefault();
    }

    public async Task<IEnumerable<Show>> GetShowPage(int skip, int take, string sortField, string sortOrder,
        IEnumerable<Filter> filters)
    {
        var sortingOnSeasonRequested = sortField == "seasons";
        var sortingOnEpisodesRequested= sortField == "episodes";
        sortField = sortingOnSeasonRequested || sortingOnEpisodesRequested ? string.Empty : sortField;
        
        var query = ShowExtensions.GenerateShowPageQuery(filters, sortField, sortOrder);
        
        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();

        var list = await connection.QueryAsync<Show, Genre, Season, Episode, Show>(query,
            (s, g, se, e) =>
            {
                s.Genres ??= new List<Genre>();
                s.Seasons ??= new List<Season>();
                se.Episodes ??= new List<Episode>();

                s.Genres.AddIfNotNull(g);
                se.Episodes.AddIfNotNull(e);
                s.Seasons.AddIfNotNull(se);
                return s;
            });

        var shows = MapShows(list).ToList();
        if (sortingOnSeasonRequested)
        {
            shows = sortOrder == "asc" 
                ? shows.OrderBy(x => x.Seasons.Count).ThenBy(x => x.SortName).ToList() 
                : shows.OrderByDescending(x => x.Seasons.Count).ThenBy(x => x.SortName).ToList() ;
        }

        if (sortingOnEpisodesRequested)
        {
            shows = MoreEnumerable.OrderBy(shows, x =>
                    x.Seasons.SelectMany(y => y.Episodes).Count(y => y.LocationType == LocationType.Disk) /
                    (double) x.Seasons.SelectMany(y => y.Episodes).Count(), sortOrder.Trim() == "asc" ? OrderByDirection.Ascending : OrderByDirection.Descending)
                .ThenBy(x => x.SortName)
                .ToList();
        }
        
        return shows.Skip(skip).Take(take);
    }

    private static IEnumerable<Show> MapShows(IEnumerable<Show> list)
    {
        var result = list
            .GroupBy(s => s.Id)
            .Select(g =>
            {
                var groupedShow = g.First();

                var groupedSeasons = g
                    .SelectMany(x => x.Seasons)
                    .GroupBy(x => x.Id)
                    .Select(x =>
                    {
                        var season = x.First();
                        season.Episodes = Enumerable
                            .DistinctBy(x.Select(y => y.Episodes.SingleOrDefault())
                                    .Where(y => y != null), y => y.Id).ToList();
                        return season;
                    });

                groupedShow.Seasons = groupedSeasons.ToList();
                var genres = g.Select(p => p.Genres?.SingleOrDefault()).Where(x => x != null).ToList();
                if (genres.Any())
                {
                    groupedShow.Genres = Enumerable.DistinctBy(genres, x => x.Id).ToList();
                }

                return groupedShow;
            });
        return result;
    }

    public async Task<Dictionary<Show, int>> GetShowsWithMostEpisodes(int count)
    {
        var query = $@"SELECT s.*, (
	SELECT COUNT(*)
	FROM {Constants.Tables.Seasons} AS s1
	INNER JOIN {Constants.Tables.Episodes} AS e0 ON s1.Id = e0.SeasonId
	WHERE (s.Id = s1.ShowId) AND (e0.LocationType = 0)) AS c
FROM {Constants.Tables.Shows} AS s
WHERE 1=1 
ORDER BY c DESC
LIMIT {count}";

        _logger.LogDebug("{Query}", query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();

        var list = await connection.QueryAsync<Show, long, (Show, int)>(query,
            (show, c) => new ValueTuple<Show, int>(show, Convert.ToInt32(c)), splitOn: "c");

        return list.ToDictionary(t => t.Item1, t => t.Item2);
    }

    public async Task DeleteAll()
    {
        _context.Shows.RemoveRange(_context.Shows);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUnwantedShows(IEnumerable<string> libraryIds)
    {
        _context.Shows.RemoveRange(_context.Shows.Where(x => !libraryIds.Any(y => y == x.LibraryId)));
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Episodes

    public Task<int> GetEpisodeCount(LocationType locationType)
    {
        return _context.Episodes.CountAsync(x => x.LocationType == locationType);
    }

    public Task<long> GetTotalRunTimeTicks()
    {
        return _context.Episodes
            .Where(x => x.LocationType == LocationType.Disk)
            .SumAsync(x => x.RunTimeTicks ?? 0);
    }

    public Task<double> GetTotalDiskSpaceUsed()
    {
        return _context.Episodes
            .Include(x => x.MediaSources)
            .Where(x => x.LocationType == LocationType.Disk)
            .SumAsync(x => x.MediaSources.Any() ? x.MediaSources.First().SizeInMb : 0d);
    }

    public IEnumerable<LabelValuePair> CalculateGenreFilterValues()
    {
        return _context.Shows
            .SelectMany(x => x.Genres)
            .Select(x => new LabelValuePair { Value = x.Name, Label = x.Name })
            .Distinct()
            .OrderBy(x => x.Label);
    }

    #endregion

    #region Helpers

    private async Task<IEnumerable<T>> ExecuteListQueryWithLibraryIds<T>(string query)
    {
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        return connection.Query<T>(query);
    }

    #endregion
}