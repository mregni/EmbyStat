using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace EmbyStat.Repositories
{
    public class SqlShowRepository : IShowRepository
    {
        private readonly SqlLiteDbContext _context;
        private readonly ISqliteBootstrap _sqliteBootstrap;

        public SqlShowRepository(SqlLiteDbContext context, ISqliteBootstrap sqliteBootstrap)
        {
            _context = context;
            _sqliteBootstrap = sqliteBootstrap;
        }

        public IEnumerable<SqlMedia> GetNewestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Shows.GetNewestPremieredMedia(libraryIds, count);
        }

        public IEnumerable<SqlMedia> GetLatestAddedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Shows.GetLatestAddedMedia(libraryIds, count);
        }

        public IEnumerable<SqlMedia> GetOldestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Shows.GetOldestPremieredMedia(libraryIds, count);
        }

        public IEnumerable<SqlExtra> GetHighestRatedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Shows.GetHighestRatedMedia(libraryIds, count);
        }

        public IEnumerable<SqlExtra> GetLowestRatedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Shows.GetLowestRatedMedia(libraryIds, count);
        }

        public async Task<Dictionary<string, int>> GetGenreChartValues(IReadOnlyList<string> libraryIds)
        {
            var query = $@"SELECT g.Name G, COUNT(*) Count
FROM {Constants.Tables.Shows} AS s
INNER JOIN {Constants.Tables.GenreShow} as gs ON (s.Id = gs.ShowsId)
INNER JOIN {Constants.Tables.Genres} as g ON (g.Id = gs.GenresId)
GROUP BY g.Name
ORDER BY Count";
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return connection.Query(query, new { Ids = libraryIds })
                .ToDictionary(
                    row => (string)row.G,
                    row => (int)row.Count);
        }

        public IEnumerable<float?> GetCommunityRatings(IReadOnlyList<string> libraryIds)
        {
            return _context.Shows
                .FilterOnLibrary(libraryIds)
                .Select(x => x.CommunityRating);
        }

        public IEnumerable<DateTime?> GetPremiereYears(IReadOnlyList<string> libraryIds)
        {
            return _context.Shows
                .FilterOnLibrary(libraryIds)
                .Select(x => x.PremiereDate);
        }

        public async Task<Dictionary<string, int>> GetOfficialRatingChartValues(IReadOnlyList<string> libraryIds)
        {
            var query = $@"SELECT upper(s.OfficialRating) OfficialRating, COUNT(*) Count
FROM {Constants.Tables.Shows} AS s
WHERE s.OfficialRating IS NOT NULL {libraryIds.AddLibraryIdFilterAsAnd("s")}
GROUP BY upper(s.OfficialRating)
ORDER BY Count";
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return connection.Query(query, new { Ids = libraryIds })
                .ToDictionary(
                    row => (string)row.OfficialRating,
                    row => (int)row.Count);
        }

        public async Task<Dictionary<string, int>> GetShowStatusCharValues(IReadOnlyList<string> libraryIds)
        {
            var query = $@"SELECT s.Status Status, COUNT(*) Count
FROM {Constants.Tables.Shows} AS s
WHERE s.Status IS NOT NULL {libraryIds.AddLibraryIdFilterAsAnd("s")}
GROUP BY s.Status
ORDER BY Count";

            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return connection.Query(query, new { Ids = libraryIds })
                .ToDictionary(
                    row => (string)row.Status,
                    row => (int)row.Count);
        }

        public async Task<IEnumerable<double>> GetCollectedRateChart(IReadOnlyList<string> libraryIds)
        {
            var query = $@"SELECT ROUND(CAST(COUNT(*) FILTER(WHERE e.LocationType = 0) AS FLOAT) / COUNT(*), 2) AS pct
FROM {Constants.Tables.Shows} AS s
INNER JOIN {Constants.Tables.Seasons} AS se ON (s.Id = se.ShowId)
INNER JOIN {Constants.Tables.Episodes} AS e ON (se.Id = e.SeasonId)
WHERE se.IndexNumber != 0 {libraryIds.AddLibraryIdFilterAsAnd("s")}
GROUP BY s.Id
ORDER BY s.Id";

            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return connection.Query<double>(query, new {Ids = libraryIds});
        }

        public Task<int> Count(IReadOnlyList<string> libraryIds)
        {
            return Count(Array.Empty<Filter>(), libraryIds);
        }

        public async Task<int> Count(Filter[] filters, IReadOnlyList<string> libraryIds)
        {
            var query = _context.Shows.GenerateCountQuery(filters, libraryIds);

            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            var result = await connection.QueryFirstAsync<int>(query, new { Ids = libraryIds });

            return result;
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

        public async Task<int> GetGenreCount(IReadOnlyList<string> libraryIds)
        {
            var query = $@"SELECT COUNT(DISTINCT g.Name)
FROM {Constants.Tables.Shows} AS s
INNER JOIN {Constants.Tables.GenreShow} AS gs ON (s.Id = gs.ShowsId)
INNER JOIN {Constants.Tables.Genres} AS g On (g.Id = gs.GenresId)
WHERE 1=1 {libraryIds.AddLibraryIdFilter("s")}";

            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return await connection.QueryFirstAsync<int>(query, new { Ids = libraryIds });
        }

        public int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type)
        {
            return _context.Shows
                .Include(x => x.People)
                .FilterOnLibrary(libraryIds)
                .SelectMany(x => x.People)
                .Distinct()
                .Count(x => x.Type == type);
        }

        public IEnumerable<string> GetMostFeaturedPersons(IReadOnlyList<string> libraryIds, PersonType type, int count)
        {
            return _context.Shows
                .Include(x => x.People)
                .ThenInclude(x => x.Person)
                .FilterOnLibrary(libraryIds)
                .SelectMany(x => x.People)
                .Where(x => x.Type == type)
                .GroupBy(x => x.Person.Name, (name, people) => new { Name = name, Count = people.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Name)
                .Take(count);
        }

        #region Shows

        public async Task UpsertShows(IEnumerable<SqlShow> shows)
        {
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();

            var showList = shows.ToList();
            await using var deleteTransaction = connection.BeginTransaction();
            var deleteQuery = "DELETE FROM Shows WHERE Id IN @Ids";
            await connection.ExecuteAsync(deleteQuery, new { Ids = showList.Select(x => x.Id) }, deleteTransaction);
            await deleteTransaction.CommitAsync();

            foreach (var show in showList)
            {
                await using var transaction = connection.BeginTransaction();

                var showQuery =
                    $@"INSERT OR REPLACE INTO {Constants.Tables.Shows} (Id,DateCreated,Banner,Logo,""Primary"",Thumb,Name,Path,PremiereDate,ProductionYear,SortName,CollectionId,CommunityRating,IMDB,TMDB,TVDB,RunTimeTicks,OfficialRating,CumulativeRunTimeTicks,Status,ExternalSynced,SizeInMb)
VALUES (@Id,@DateCreated,@Banner,@Logo,@Primary,@Thumb,@Name,@Path,@PremiereDate,@ProductionYear,@SortName,@CollectionId,@CommunityRating,@IMDB,@TMDB,@TVDB,@RunTimeTicks,@OfficialRating,@CumulativeRunTimeTicks,@Status,@ExternalSynced,@SizeInMb)";
                await connection.ExecuteAsync(showQuery, show, transaction);

                if (show.Seasons.AnyNotNull())
                {
                    var seasonQuery =
                        $@"INSERT OR REPLACE INTO {Constants.Tables.Seasons} (Id,DateCreated,Banner,Logo,""Primary"",Thumb,Name,Path,PremiereDate,ProductionYear,SortName,CollectionId,IndexNumber,IndexNumberEnd,LocationType,ShowId)
VALUES (@Id,@DateCreated,@Banner,@Logo,@Primary,@Thumb,@Name,@Path,@PremiereDate,@ProductionYear,@SortName,@CollectionId,@IndexNumber,@IndexNumberEnd,@LocationType,@ShowId)";
                    await connection.ExecuteAsync(seasonQuery, show.Seasons, transaction);
                }

                if (show.Genres.AnyNotNull())
                {
                    var genreQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.GenreShow} (GenresId, ShowsId) 
VALUES (@GenreId, @ShowsId)";
                    var genreList = show.Genres.Select(x => new { GenreId = x.Id, ShowsId = show.Id });
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
                        @$"INSERT OR REPLACE INTO {Constants.Tables.Episodes} (Id,DateCreated,Banner,Logo,""Primary"",Thumb,Name,Path,PremiereDate,ProductionYear,SortName,CollectionId,Container,CommunityRating,IMDB,TMDB,TVDB,RunTimeTicks,OfficialRating,Video3DFormat,DvdEpisodeNumber,DvdSeasonNumber,IndexNumber,IndexNumberEnd,SeasonId,LocationType)
VALUES (@Id,@DateCreated,@Banner,@Logo,@Primary,@Thumb,@Name,@Path,@PremiereDate,@ProductionYear,@SortName,@CollectionId,@Container,@CommunityRating,@IMDB,@TMDB,@TVDB,@RunTimeTicks,@OfficialRating,@Video3DFormat,@DvdEpisodeNumber,@DvdSeasonNumber,@IndexNumber,@IndexNumberEnd,@SeasonId,@LocationType)";
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

        public async Task<IEnumerable<SqlShow>> GetAllShowsWithEpisodes(IReadOnlyList<string> libraryIds)
        {
            var query = _context.Shows.GenerateFullShowQuery(true, libraryIds);
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();

            var list = await connection.QueryAsync<SqlShow, SqlSeason, SqlEpisode, SqlShow>(query, (s, se, e) =>
            {
                s.Seasons ??= new List<SqlSeason>();
                se.Episodes ??= new List<SqlEpisode>();

                se.Episodes.AddIfNotNull(e);
                s.Seasons.AddIfNotNull(se);
                return s;
            }, new { Ids = libraryIds });

            return MapShows(list);
        }

        private static IEnumerable<SqlShow> MapShows(IEnumerable<SqlShow> list)
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
                    groupedShow.Genres = Enumerable.DistinctBy(g.Select(p => p.Genres.SingleOrDefault()).Where(x => x != null), x => x.Id).ToList();
                    return groupedShow;
                });
            return result;
        }

        public async Task<SqlShow> GetShowByIdWithEpisodes(string showId)
        {
            var query = _context.Shows.GenerateFullShowQuery(true, Array.Empty<string>());
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();

            var list = await connection.QueryAsync<SqlShow, SqlSeason, SqlEpisode, SqlShow>(query, (s, se, e) =>
            {
                s.Seasons ??= new List<SqlSeason>();
                se.Episodes ??= new List<SqlEpisode>();

                se.Episodes.AddIfNotNull(e);
                s.Seasons.AddIfNotNull(se);
                return s;
            }, new { Id = showId });


            return MapShows(list).FirstOrDefault();
        }

        public void RemoveShows()
        {
            _context.Shows.RemoveRange(_context.Shows);
        }

        public Dictionary<SqlShow, int> GetShowsWithMostEpisodes(IReadOnlyList<string> libraryIds, int count)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SqlShow>> GetShowPage(int skip, int take, string sortField, string sortOrder, Filter[] filters, List<string> libraryIds)
        {
            var query = _context.Shows.GenerateShowPageQuery(filters, libraryIds, sortField, sortOrder);
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();

            var list = await connection.QueryAsync<SqlShow, SqlGenre, SqlSeason, SqlEpisode, SqlShow>(query, (s, g, se, e) =>
            {
                s.Genres ??= new List<SqlGenre>();
                s.Seasons ??= new List<SqlSeason>();
                se.Episodes ??= new List<SqlEpisode>();

                s.Genres.AddIfNotNull(g);
                se.Episodes.AddIfNotNull(e);
                s.Seasons.AddIfNotNull(se);
                return s;
            }, new { Ids = libraryIds });

            return MapShows(list);
        }

        #endregion

        #region Episodes

        public IEnumerable<SqlEpisode> GetAllEpisodesForShow(string showId)
        {
            return _context.Shows
                .Include(x => x.Seasons)
                .ThenInclude(x => x.Episodes)
                .Where(x => x.Id == showId)
                .SelectMany(x => x.Seasons)
                .SelectMany(x => x.Episodes);
        }

        public Task<int> GetEpisodeCount(IReadOnlyList<string> libraryIds, LocationType locationType)
        {
            return _context.Episodes.CountAsync(x => x.LocationType == locationType);
        }

        public Task<long> GetTotalRunTimeTicks(IReadOnlyList<string> libraryIds)
        {
            return _context.Episodes
                .Where(x => x.LocationType == LocationType.Disk)
                .SumAsync(x => x.RunTimeTicks ?? 0);
        }

        public Task<double> GetTotalDiskSpaceUsed(IReadOnlyList<string> libraryIds)
        {
            return _context.Episodes
                .Include(x => x.MediaSources)
                .Where(x => x.LocationType == LocationType.Disk)
                .SumAsync(x => x.MediaSources.Any() ? x.MediaSources.First().SizeInMb : 0d);
        }

        #endregion
    }
}
