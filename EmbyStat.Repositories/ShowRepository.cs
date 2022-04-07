using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace EmbyStat.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly EsDbContext _context;
        private readonly ISqliteBootstrap _sqliteBootstrap;

        public ShowRepository(EsDbContext context, ISqliteBootstrap sqliteBootstrap)
        {
            _context = context;
            _sqliteBootstrap = sqliteBootstrap;
        }

        public async Task<IEnumerable<Media>> GetNewestPremieredMedia(int count)
        {
            var query = _context.Shows.GenerateGetPremieredListQuery(count, "DESC", Constants.Tables.Shows);
            return await ExecuteListQueryWithLibraryIds<Show>(query);
        }

        public async Task<IEnumerable<Media>> GetOldestPremieredMedia(int count)
        {
            var query = _context.Shows.GenerateGetPremieredListQuery(count, "ASC", Constants.Tables.Shows);
            return await ExecuteListQueryWithLibraryIds<Show>(query);
        }

        public async Task<IEnumerable<Extra>> GetHighestRatedMedia(int count)
        {
            var query = _context.Shows.GenerateGetCommunityRatingListQuery(count, "DESC", Constants.Tables.Shows);
            return await ExecuteListQueryWithLibraryIds<Show>(query);
        }

        public async Task<IEnumerable<Extra>> GetLowestRatedMedia(int count)
        {
            var query = _context.Shows.GenerateGetCommunityRatingListQuery(count, "ASC", Constants.Tables.Shows);
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
            var query = _context.Shows.GenerateCountQuery(filters);

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

        public async Task<int> CompleteCollectedCount()
        {
            var query = $@"SELECT COUNT(*) Count 
FROM {Constants.Tables.Shows} AS s 
WHERE NOT EXISTS (SELECT 1 FROM {Constants.Tables.Seasons} AS se INNER JOIN {Constants.Tables.Episodes} AS ep ON (se.Id = ep.SeasonId) 
    WHERE se.ShowId = s.Id AND ep.LocationType = 1) ";

            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return connection.QueryFirst<int>(query);
        }

        public bool Any()
        {
            return _context.Shows.Any();
        }

        public int GetMediaCountForPerson(string name, string genre)
        {
            throw new NotImplementedException();
        }

        public int GetMediaCountForPerson(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetGenreCount()
        {
            var query = $@"SELECT COUNT(DISTINCT g.Name)
FROM {Constants.Tables.Shows} AS s
INNER JOIN {Constants.Tables.GenreShow} AS gs ON (s.Id = gs.ShowsId)
INNER JOIN {Constants.Tables.Genres} AS g On (g.Id = gs.GenresId)
WHERE 1=1 ";

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

        public IEnumerable<string> GetMostFeaturedPersons(PersonType type, int count)
        {
            return _context.Shows
                .Include(x => x.People)
                .ThenInclude(x => x.Person)
                .SelectMany(x => x.People)
                .Where(x => x.Type == type)
                .GroupBy(x => x.Person.Name, (name, people) => new {Name = name, Count = people.Count()})
                .OrderByDescending(x => x.Count)
                .Select(x => x.Name)
                .Take(count);
        }

        #region Shows

        public async Task UpsertShows(IEnumerable<Show> shows)
        {
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();

            var showList = shows.ToList();
            await using var deleteTransaction = connection.BeginTransaction();
            var deleteQuery = "DELETE FROM Shows WHERE Id IN @Ids";
            await connection.ExecuteAsync(deleteQuery, new {Ids = showList.Select(x => x.Id)}, deleteTransaction);
            await deleteTransaction.CommitAsync();

            foreach (var show in showList)
            {
                await using var transaction = connection.BeginTransaction();

                var showQuery =
                    $@"INSERT OR REPLACE INTO {Constants.Tables.Shows} (Id,DateCreated,Banner,Logo,""Primary"",Thumb,Name,Path,PremiereDate,ProductionYear,SortName,CommunityRating,IMDB,TMDB,TVDB,RunTimeTicks,OfficialRating,CumulativeRunTimeTicks,Status,ExternalSynced,SizeInMb)
VALUES (@Id,@DateCreated,@Banner,@Logo,@Primary,@Thumb,@Name,@Path,@PremiereDate,@ProductionYear,@SortName,@CommunityRating,@IMDB,@TMDB,@TVDB,@RunTimeTicks,@OfficialRating,@CumulativeRunTimeTicks,@Status,@ExternalSynced,@SizeInMb)";
                await connection.ExecuteAsync(showQuery, show, transaction);

                if (show.Seasons.AnyNotNull())
                {
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
                    show.People.ForEach(x => x.ShowId = show.Id);
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
                            episode.MediaSources.ForEach(x => x.EpisodeId = episode.Id);
                            await connection.ExecuteAsync(mediaSourceQuery, episode.MediaSources, transaction);
                        }

                        if (episode.VideoStreams.AnyNotNull())
                        {
                            var videoStreamQuery =
                                @$"INSERT OR REPLACE INTO {Constants.Tables.VideoStreams} (Id,AspectRatio,AverageFrameRate,BitRate,Channels,Height,Language,Width,BitDepth,Codec,IsDefault,VideoRange,EpisodeId) 
VALUES (@Id,@AspectRatio,@AverageFrameRate,@BitRate,@Channels,@Height,@Language,@Width,@BitDepth,@Codec,@IsDefault,@VideoRange,@EpisodeId)";
                            episode.VideoStreams.ForEach(x => x.EpisodeId = episode.Id);
                            await connection.ExecuteAsync(videoStreamQuery, episode.VideoStreams, transaction);
                        }

                        if (episode.AudioStreams.AnyNotNull())
                        {
                            var audioStreamQuery =
                                @$"INSERT OR REPLACE INTO {Constants.Tables.AudioStreams} (Id,BitRate,ChannelLayout,Channels,Codec,Language,SampleRate,IsDefault,EpisodeId)
VALUES (@Id,@BitRate,@ChannelLayout,@Channels,@Codec,@Language,@SampleRate,@IsDefault,@EpisodeId)";
                            episode.AudioStreams.ForEach(x => x.EpisodeId = episode.Id);
                            await connection.ExecuteAsync(audioStreamQuery, episode.AudioStreams, transaction);
                        }

                        if (episode.SubtitleStreams.AnyNotNull())
                        {
                            var subtitleStreamQuery =
                                @$"INSERT OR REPLACE INTO {Constants.Tables.SubtitleStreams} (Id,Codec,DisplayTitle,IsDefault,Language,EpisodeId)
VALUES (@Id,@Codec,@DisplayTitle,@IsDefault,@Language,@EpisodeId)";
                            episode.SubtitleStreams.ForEach(x => x.EpisodeId = episode.Id);
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
            var query = _context.Shows.GenerateFullShowQuery();
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
            var query = _context.Shows.GenerateFullShowWithGenresQuery();
            query += $" AND s.Id = {showId}";

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
                }, new {Id = showId});

            return MapShows(list).FirstOrDefault();
        }

        public async Task<IEnumerable<Show>> GetShowPage(int skip, int take, string sortField, string sortOrder,
            Filter[] filters)
        {
            var query = _context.Shows.GenerateShowPageQuery(filters, sortField, sortOrder);
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

            return MapShows(list).Skip(skip).Take(take);
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
                                .DistinctBy(x.Select(y => y.Episodes.SingleOrDefault()).Where(x => x != null),
                                    x => x.Id).ToList();
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

        public void RemoveShows()
        {
            _context.Shows.RemoveRange(_context.Shows);
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
}