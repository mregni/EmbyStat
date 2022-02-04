using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Movies;
using EmbyStat.Common.SqLite.Streams;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;

namespace EmbyStat.Repositories
{
    public class SqlMovieRepository : IMovieRepository
    {
        private readonly SqlLiteDbContext _context;
        private readonly ISqliteBootstrap _sqliteBootstrap;
        public SqlMovieRepository(SqlLiteDbContext context, ISqliteBootstrap sqliteBootstrap)
        {
            _context = context;
            _sqliteBootstrap = sqliteBootstrap;
        }

        public void RemoveAll()
        {
            _context.Movies.RemoveRange(_context.Movies);
            _context.SaveChanges();
        }

        public async Task UpsertRange(IEnumerable<SqlMovie> movies)
        {
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();

            foreach (var movie in movies)
            {
                try
                {
                    await using var transaction = connection.BeginTransaction();
                    await connection.ExecuteAsync($"DELETE FROM {Constants.Tables.MediaSources} WHERE MovieId = @Id", movie, transaction);
                    await connection.ExecuteAsync($"DELETE FROM {Constants.Tables.VideoStreams} WHERE MovieId = @Id", movie, transaction);
                    await connection.ExecuteAsync($"DELETE FROM {Constants.Tables.AudioStreams} WHERE MovieId = @Id", movie, transaction);
                    await connection.ExecuteAsync($"DELETE FROM {Constants.Tables.SubtitleStreams} WHERE MovieId = @Id", movie, transaction);

                    var movieQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.Movies} (Id,DateCreated,Banner,Logo,""Primary"",Thumb,Name,ParentId,Path,PremiereDate,ProductionYear,SortName,CollectionId,OriginalTitle,Container,MediaType,CommunityRating,IMDB,TMDB,TVDB,RunTimeTicks,OfficialRating,Video3DFormat)
VALUES (@Id,@DateCreated,@Banner,@Logo,@Primary,@Thumb,@Name,@ParentId,@Path,@PremiereDate,@ProductionYear,@SortName,@CollectionId,@OriginalTitle,@Container,@MediaType,@CommunityRating,@IMDB,@TMDB,@TVDB,@RunTimeTicks,@OfficialRating,@Video3DFormat)";
                    await connection.ExecuteAsync(movieQuery, movie, transaction);

                    if (movie.MediaSources.AnyNotNull())
                    {
                        var mediaSourceQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.MediaSources} (Id,BitRate,Container,Path,Protocol,RunTimeTicks,SizeInMb,MovieId) 
VALUES (@Id, @BitRate,@Container,@Path,@Protocol,@RunTimeTicks,@SizeInMb,@MovieId)";
                        movie.MediaSources.ForEach(x => x.MovieId = movie.Id);
                        await connection.ExecuteAsync(mediaSourceQuery, movie.MediaSources, transaction);
                    }

                    if (movie.VideoStreams.AnyNotNull())
                    {
                        var videoStreamQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.VideoStreams} (Id,AspectRatio,AverageFrameRate,BitRate,Channels,Height,Language,Width,BitDepth,Codec,IsDefault,VideoRange,MovieId) 
VALUES (@Id,@AspectRatio,@AverageFrameRate,@BitRate,@Channels,@Height,@Language,@Width,@BitDepth,@Codec,@IsDefault,@VideoRange,@MovieId)";
                        movie.VideoStreams.ForEach(x => x.MovieId = movie.Id);
                        await connection.ExecuteAsync(videoStreamQuery, movie.VideoStreams, transaction);
                    }

                    if (movie.AudioStreams.AnyNotNull())
                    {
                        var audioStreamQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.AudioStreams} (Id,BitRate,ChannelLayout,Channels,Codec,Language,SampleRate,IsDefault,MovieId)
VALUES (@Id,@BitRate,@ChannelLayout,@Channels,@Codec,@Language,@SampleRate,@IsDefault,@MovieId)";
                        movie.AudioStreams.ForEach(x => x.MovieId = movie.Id);
                        await connection.ExecuteAsync(audioStreamQuery, movie.AudioStreams, transaction);
                    }

                    if (movie.SubtitleStreams.AnyNotNull())
                    {
                        var subtitleStreamQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.SubtitleStreams} (Id,Codec,DisplayTitle,IsDefault,Language,MovieId)
VALUES (@Id,@Codec,@DisplayTitle,@IsDefault,@Language,@MovieId)";
                        movie.SubtitleStreams.ForEach(x => x.MovieId = movie.Id);
                        await connection.ExecuteAsync(subtitleStreamQuery, movie.SubtitleStreams, transaction);
                    }

                    if (movie.Genres.AnyNotNull())
                    {
                        var genreQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.GenreMovie} (GenresId, MoviesId) 
VALUES ((SELECT Id FROM Genres WHERE name = @GenreName), @MovieId)";
                        var genreList = movie.Genres.Select(x => new { GenreName = x.Name, MovieId = movie.Id });
                        await connection.ExecuteAsync(genreQuery, genreList, transaction);
                    }

                    if (movie.People.AnyNotNull())
                    {
                        var peopleQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.MediaPerson} (Type, MovieId, PersonId)
VALUES (@Type, @MovieId, @PersonId)";
                        movie.People.ForEach(x => x.MovieId = movie.Id);
                        await connection.ExecuteAsync(peopleQuery, movie.People, transaction);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }

        public IEnumerable<SqlMovie> GetAll(IReadOnlyList<string> libraryIds)
        {
            return GetAll(libraryIds, false);
        }

        public IEnumerable<SqlMovie> GetAll(IReadOnlyList<string> libraryIds, bool includeGenres)
        {
            var query = _context.Movies.FilterOnLibrary(libraryIds);

            if (includeGenres)
            {
                query = query.Include(x => x.Genres);
            }

            return query.OrderBy(x => x.SortName);
        }

        public IEnumerable<SqlMovie> GetAllWithImdbId(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Where(x => x.IMDB != null)
                .OrderBy(x => x.SortName);
        }

        public SqlMovie GetById(string id)
        {
            return _context.Movies
                .Include(x => x.Genres)
                .Include(x => x.AudioStreams)
                .Include(x => x.MediaSources)
                .Include(x => x.SubtitleStreams)
                .Include(x => x.VideoStreams)
                .Include(x => x.Genres)
                .FirstOrDefault(x => x.Id == id);
        }


        public long? GetTotalRuntime(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Sum(x => x.RunTimeTicks);
        }

        public IEnumerable<SqlMovie> GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks, int count)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Where(x => x.RunTimeTicks != null && x.RunTimeTicks > toShortMovieTicks)
                .OrderBy(x => x.RunTimeTicks)
                .Take(count);
        }

        public IEnumerable<SqlMovie> GetLongestMovie(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Where(x => x.RunTimeTicks != null)
                .OrderByDescending(x => x.RunTimeTicks)
                .Take(count);
        }

        public double GetTotalDiskSpace(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .SelectMany(x => x.MediaSources)
                .Sum(x => x.SizeInMb);
        }

        public IEnumerable<SqlMovie> GetToShortMovieList(IReadOnlyList<string> libraryIds, int toShortMovieMinutes)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Where(x => x.RunTimeTicks < TimeSpan.FromMinutes(toShortMovieMinutes).Ticks)
                .OrderBy(x => x.SortName);
        }

        public IEnumerable<SqlMovie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Where(x => x.IMDB == null)
                .OrderBy(x => x.SortName);
        }

        public IEnumerable<SqlMovie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Where(x => x.Primary == null)
                .OrderBy(x => x.SortName);
        }

        public async Task<IEnumerable<SqlMovie>> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters, List<string> libraryIds)
        {
            var query = _context.Movies.GenerateFullMovieQuery(filters, libraryIds, sortField, sortOrder);
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();

            var list = await connection.QueryAsync<SqlMovie, SqlGenre, SqlAudioStream, SqlVideoStream, SqlSubtitleStream, SqlMediaSource, SqlMovie>(query, (m, g, aus, vis, sus, mes) =>
            {
                m.Genres ??= new List<SqlGenre>();
                m.AudioStreams ??= new List<SqlAudioStream>();
                m.VideoStreams ??= new List<SqlVideoStream>();
                m.SubtitleStreams ??= new List<SqlSubtitleStream>();
                m.MediaSources ??= new List<SqlMediaSource>();

                m.Genres.AddIfNotNull(g);
                m.AudioStreams.AddIfNotNull(aus);
                m.VideoStreams.AddIfNotNull(vis);
                m.SubtitleStreams.AddIfNotNull(sus);
                m.MediaSources.AddIfNotNull(mes);
                return m;
            }, new { Ids = libraryIds });

            var result = list
                .GroupBy(m => m.Id)
                .Select(g =>
            {
                var groupedMovie = g.First();
                groupedMovie.Genres = Enumerable.DistinctBy(g.Select(p => p.Genres.SingleOrDefault()).Where(x => x != null), x => x.Id).ToList();
                groupedMovie.AudioStreams = Enumerable.DistinctBy(g.Select(p => p.AudioStreams.SingleOrDefault()).Where(x => x != null), x => x.Id).ToList();
                groupedMovie.VideoStreams = Enumerable.DistinctBy(g.Select(p => p.VideoStreams.SingleOrDefault()).Where(x => x != null), x => x.Id).ToList();
                groupedMovie.SubtitleStreams = Enumerable.DistinctBy(g.Select(p => p.SubtitleStreams.SingleOrDefault()).Where(x => x != null), x => x.Id).ToList();
                groupedMovie.MediaSources = Enumerable.DistinctBy(g.Select(p => p.MediaSources.SingleOrDefault()).Where(x => x != null), x => x.Id).ToList();
                return groupedMovie;
            })
                .Skip(skip)
                .Take(take);

            return result;
        }



        public IEnumerable<LabelValuePair> CalculateSubtitleFilterValues(IReadOnlyList<string> libraryIds)
        {
            return Enumerable.DistinctBy(_context.Movies
                    .Include(x => x.SubtitleStreams)
                    .FilterOnLibrary(libraryIds)
                    .SelectMany(x => x.SubtitleStreams)
                    .Where(x => x.Language != "und" && x.Language != "Und" && x.Language != null)
                    .AsEnumerable()
                    .Select(x => new LabelValuePair { Value = x.Language, Label = x.DisplayTitle.Split('(')[0] }), x => x.Label)
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateContainerFilterValues(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Select(x => new LabelValuePair { Value = x.Container, Label = x.Container })
                .Distinct()
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateGenreFilterValues(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .SelectMany(x => x.Genres)
                .Select(x => new LabelValuePair { Value = x.Name, Label = x.Name })
                .Distinct()
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateCollectionFilterValues()
        {
            //TODO: safe collections somewhere so we can display names in the dropdown
            //not working at the moment, will display Id's
            return _context.Movies
                .Select(x => new LabelValuePair { Value = x.CollectionId, Label = x.CollectionId })
                .Distinct()
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateCodecFilterValues(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .Include(x => x.VideoStreams)
                .FilterOnLibrary(libraryIds)
                .Where(x => x.VideoStreams.Any())
                .Select(x => new LabelValuePair
                {
                    Value = x.VideoStreams.First().Codec ?? string.Empty,
                    Label = x.VideoStreams.First().Codec ?? string.Empty
                })
                .Distinct()
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateVideoRangeFilterValues(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .Include(x => x.VideoStreams)
                .FilterOnLibrary(libraryIds)
                .Where(x => x.VideoStreams.Any())
                .Select(x => new LabelValuePair
                {
                    Value = x.VideoStreams.First().VideoRange ?? string.Empty,
                    Label = x.VideoStreams.First().VideoRange ?? string.Empty
                })
                .Distinct()
                .OrderBy(x => x.Label);
        }

        public IEnumerable<SqlMedia> GetLatestAddedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Movies.GetLatestAddedMedia(libraryIds, count);
        }

        public IEnumerable<SqlMedia> GetNewestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Movies.GetNewestPremieredMedia(libraryIds, count);
        }

        public IEnumerable<SqlMedia> GetOldestPremieredMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Movies.GetOldestPremieredMedia(libraryIds, count);
        }

        public IEnumerable<SqlExtra> GetHighestRatedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Movies.GetHighestRatedMedia(libraryIds, count);
        }

        public IEnumerable<SqlExtra> GetLowestRatedMedia(IReadOnlyList<string> libraryIds, int count)
        {
            return _context.Movies.GetLowestRatedMedia(libraryIds, count);
        }

        public async Task<Dictionary<string, int>> GetMovieGenreChartValues(IReadOnlyList<string> libraryIds)
        {
            var query = $@"SELECT g.Name G, COUNT(*) Count
FROM {Constants.Tables.Movies} AS m
INNER JOIN {Constants.Tables.GenreMovie} as gm ON (m.Id = gm.MoviesId)
INNER JOIN {Constants.Tables.Genres} as g ON (g.Id = gm.GenresId)
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
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Select(x => x.CommunityRating);
        }

        public IEnumerable<DateTime?> GetPremiereYears(IReadOnlyList<string> libraryIds)
        {
            return _context.Movies
                .FilterOnLibrary(libraryIds)
                .Select(x => x.PremiereDate);
        }

        public async Task<Dictionary<string, int>> GetOfficialRatingChartValues(IReadOnlyList<string> libraryIds)
        {
            var query = $@"SELECT upper(m.OfficialRating) OfficialRating, COUNT(*) Count
FROM {Constants.Tables.Movies} AS m
WHERE m.OfficialRating IS NOT NULL {libraryIds.AddLibraryIdFilterAsAnd("m")}
GROUP BY upper(m.OfficialRating)
ORDER BY Count";
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return connection.Query(query, new { Ids = libraryIds })
                .ToDictionary(
                row => (string)row.OfficialRating,
                row => (int)row.Count);
        }

        public Task<int> Count(IReadOnlyList<string> libraryIds)
        {
            return Count(Array.Empty<Filter>(), libraryIds);
        }

        public async Task<int> Count(Filter[] filters, IReadOnlyList<string> libraryIds)
        {
            var query = MovieExtensions.GenerateCountQuery(filters, libraryIds);

            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            var result = await connection.QueryFirstAsync<int>(query, new { Ids = libraryIds });

            return result;
        }

        public bool Any()
        {
            return _context.Movies.Any();
        }

        #region People

        public int GetMediaCountForPerson(string name, string genre)
        {
            return _context.Movies
                .Include(x => x.Genres)
                .Include(x => x.People)
                .ThenInclude(x => x.Person)
                .Count(x => x.Genres.Any(y => y.Name == genre) && x.People.Any(y => name == y.Person.Name));
        }

        public int GetMediaCountForPerson(string name)
        {
            return _context.Movies
                .Include(x => x.People)
                .ThenInclude(x => x.Person)
                .Count(x => x.People.Any(y => name == y.Person.Name));
        }

        public IEnumerable<string> GetMostFeaturedPersons(IReadOnlyList<string> libraryIds, PersonType type, int count)
        {
            return _context.Movies
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

        public int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type)
        {
            return _context.Movies
                .Include(x => x.People)
                .FilterOnLibrary(libraryIds)
                .SelectMany(x => x.People)
                .Distinct()
                .Count(x => x.Type == type);
        }

        public async Task<int> GetGenreCount(IReadOnlyList<string> libraryIds)
        {
            var query = $@"SELECT COUNT(DISTINCT g.Name)
FROM {Constants.Tables.Movies} AS m
INNER JOIN {Constants.Tables.GenreMovie} AS gm ON (m.Id = gm.MoviesId)
INNER JOIN {Constants.Tables.Genres} AS g On (g.Id = gm.GenresId)
{libraryIds.AddLibraryIdFilter("m")}";

            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return await connection.QueryFirstAsync<int>(query, new { Ids = libraryIds });
        }

        #endregion

        #region Helpers

      

        #endregion
    }
}
