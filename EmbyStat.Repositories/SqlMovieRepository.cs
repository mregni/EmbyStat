using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities.Helpers;
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

                    var movieQuery = @$"INSERT OR REPLACE INTO {Constants.Tables.Movies} (Id,DateCreated,Banner,Logo,""Primary"",Thumb,Name,Path,PremiereDate,ProductionYear,SortName,OriginalTitle,Container,CommunityRating,IMDB,TMDB,TVDB,RunTimeTicks,OfficialRating,Video3DFormat)
VALUES (@Id,@DateCreated,@Banner,@Logo,@Primary,@Thumb,@Name,@Path,@PremiereDate,@ProductionYear,@SortName,@OriginalTitle,@Container,@CommunityRating,@IMDB,@TMDB,@TVDB,@RunTimeTicks,@OfficialRating,@Video3DFormat)";
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
        
        public async Task DeleteAll()
        {
            _context.Movies.RemoveRange(_context.Movies);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<SqlMovie> GetAll()
        {
            return GetAll(false);
        }

        public IEnumerable<SqlMovie> GetAll(bool includeGenres)
        {
            var query = _context.Movies.AsQueryable();

            if (includeGenres)
            {
                query = query.Include(x => x.Genres);
            }

            return query.OrderBy(x => x.SortName);
        }

        public IEnumerable<SqlMovie> GetAllWithImdbId()
        {
            return _context.Movies
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


        public long? GetTotalRuntime()
        {
            return _context.Movies
                .Sum(x => x.RunTimeTicks);
        }

        public IEnumerable<SqlMovie> GetShortestMovie(long toShortMovieTicks, int count)
        {
            return _context.Movies
                .Where(x => x.RunTimeTicks != null && x.RunTimeTicks > toShortMovieTicks)
                .OrderBy(x => x.RunTimeTicks)
                .Take(count);
        }

        public IEnumerable<SqlMovie> GetLongestMovie(int count)
        {
            return _context.Movies
                .Where(x => x.RunTimeTicks != null)
                .OrderByDescending(x => x.RunTimeTicks)
                .Take(count);
        }

        public double GetTotalDiskSpace()
        {
            return _context.Movies
                .SelectMany(x => x.MediaSources)
                .Sum(x => x.SizeInMb);
        }

        public IEnumerable<SqlMovie> GetToShortMovieList(int toShortMovieMinutes)
        {
            return _context.Movies
                .Where(x => x.RunTimeTicks < TimeSpan.FromMinutes(toShortMovieMinutes).Ticks)
                .OrderBy(x => x.SortName);
        }

        public IEnumerable<SqlMovie> GetMoviesWithoutImdbId()
        {
            return _context.Movies
                .Where(x => x.IMDB == null)
                .OrderBy(x => x.SortName);
        }

        public IEnumerable<SqlMovie> GetMoviesWithoutPrimaryImage()
        {
            return _context.Movies
                .Where(x => x.Primary == null)
                .OrderBy(x => x.SortName);
        }
        
        public async Task<IEnumerable<SqlMovie>> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters)
        {
            var query = _context.Movies.GenerateFullMovieQuery(filters, sortField, sortOrder);
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
            });

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

        public IEnumerable<LabelValuePair> CalculateSubtitleFilterValues()
        {
            return Enumerable.DistinctBy(
                    _context.Movies
                    .Include(x => x.SubtitleStreams)
                    .SelectMany(x => x.SubtitleStreams)
                    .Where(x => x.Language != "und" && x.Language != "Und" && x.Language != null && x.Language != "Scr" && x.Language != "scr")
                    .AsEnumerable()
                    .Select(x => new LabelValuePair { Value = x.Language, Label = x.DisplayTitle.Split('(')[0] }), x => x.Label)
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateContainerFilterValues()
        {
            return _context.Movies
                .Select(x => new LabelValuePair { Value = x.Container, Label = x.Container })
                .Distinct()
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateGenreFilterValues()
        {
            return _context.Movies
                .SelectMany(x => x.Genres)
                .Select(x => new LabelValuePair { Value = x.Name, Label = x.Name })
                .Distinct()
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateCodecFilterValues()
        {
            return _context.Movies
                .Include(x => x.VideoStreams)
                .Where(x => x.VideoStreams.Any())
                .Select(x => new LabelValuePair
                {
                    Value = x.VideoStreams.First().Codec ?? string.Empty,
                    Label = x.VideoStreams.First().Codec ?? string.Empty
                })
                .Distinct()
                .OrderBy(x => x.Label);
        }

        public IEnumerable<LabelValuePair> CalculateVideoRangeFilterValues()
        {
            return _context.Movies
                .Include(x => x.VideoStreams)
                .Where(x => x.VideoStreams.Any())
                .Select(x => new LabelValuePair
                {
                    Value = x.VideoStreams.First().VideoRange ?? string.Empty,
                    Label = x.VideoStreams.First().VideoRange ?? string.Empty
                })
                .Distinct()
                .OrderBy(x => x.Label);
        }

        public IEnumerable<SqlMedia> GetLatestAddedMedia(int count)
        {
            return _context.Movies.GetLatestAddedMedia(count);
        }

        public async Task<IEnumerable<SqlMedia>> GetNewestPremieredMedia(int count)
        {
            var query = _context.Movies.GenerateGetPremieredListQuery(count, "DESC", Constants.Tables.Movies);
            return await ExecuteListQuery<SqlMovie>(query);
        }

        public async Task<IEnumerable<SqlMedia>> GetOldestPremieredMedia(int count)
        {
            var query = _context.Movies.GenerateGetPremieredListQuery(count, "ASC", Constants.Tables.Movies);
            return await ExecuteListQuery<SqlMovie>(query);
        }
        
        public async Task<IEnumerable<SqlExtra>> GetHighestRatedMedia(int count)
        {
            var query = _context.Movies.GenerateGetCommunityRatingListQuery(count, "DESC", Constants.Tables.Movies);
            return await ExecuteListQuery<SqlMovie>(query);
        }

        public async Task<IEnumerable<SqlExtra>>  GetLowestRatedMedia(int count)
        {
            var query = _context.Movies.GenerateGetCommunityRatingListQuery(count, "ASC", Constants.Tables.Movies);
            return await ExecuteListQuery<SqlMovie>(query);
        }

        public async Task<Dictionary<string, int>> GetGenreChartValues()
        {
            var query = $@"SELECT g.Name G, COUNT(*) Count
FROM {Constants.Tables.Movies} AS m
INNER JOIN {Constants.Tables.GenreMovie} as gm ON (m.Id = gm.MoviesId)
INNER JOIN {Constants.Tables.Genres} as g ON (g.Id = gm.GenresId)
GROUP BY g.Name
ORDER BY g.Name";
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return connection.Query(query)
                .ToDictionary(
                row => (string)row.G,
                row => (int)row.Count);
        }

        public IEnumerable<decimal?> GetCommunityRatings()
        {
            return _context.Movies
                .Select(x => x.CommunityRating);
        }

        public IEnumerable<DateTime?> GetPremiereYears()
        {
            return _context.Movies
                .Select(x => x.PremiereDate);
        }

        public async Task<Dictionary<string, int>> GetOfficialRatingChartValues()
        {
            var query = $@"SELECT upper(m.OfficialRating) OfficialRating, COUNT(*) Count
FROM {Constants.Tables.Movies} AS m
WHERE m.OfficialRating IS NOT NULL
GROUP BY upper(m.OfficialRating)
ORDER BY OfficialRating";
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return connection.Query(query)
                .ToDictionary(
                row => (string)row.OfficialRating,
                row => (int)row.Count);
        }

        public Task<int> Count()
        {
            return Count(Array.Empty<Filter>());
        }

        public async Task<int> Count(Filter[] filters)
        {
            var query = _context.Movies.GenerateCountQuery(filters);

            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            var result = await connection.QueryFirstAsync<int>(query);

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

        public IEnumerable<string> GetMostFeaturedPersons(PersonType type, int count)
        {
            return _context.Movies
                    .Include(x => x.People)
                    .ThenInclude(x => x.Person)
                    .SelectMany(x => x.People)
                    .Where(x => x.Type == type)
                    .GroupBy(x => x.Person.Name, (name, people) => new { Name = name, Count = people.Count() })
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Name)
                    .Take(count);
        }

        public int GetPeopleCount(PersonType type)
        {
            return _context.Movies
                .Include(x => x.People)
                .SelectMany(x => x.People)
                .Distinct()
                .Count(x => x.Type == type);
        }

        public async Task<int> GetGenreCount()
        {
            var query = $@"SELECT COUNT(DISTINCT g.Name)
FROM {Constants.Tables.Movies} AS m
INNER JOIN {Constants.Tables.GenreMovie} AS gm ON (m.Id = gm.MoviesId)
INNER JOIN {Constants.Tables.Genres} AS g On (g.Id = gm.GenresId)";

            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return await connection.QueryFirstAsync<int>(query);
        }

        #endregion

        #region Helpers

        private async Task<IEnumerable<T>> ExecuteListQuery<T>(string query)
        {
            await using var connection = _sqliteBootstrap.CreateConnection();
            await connection.OpenAsync();
            return connection.Query<T>(query);
        }

        #endregion
    }
}
