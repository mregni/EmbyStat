using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Repositories.Interfaces.Helpers;
using Microsoft.EntityFrameworkCore;

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

        public Task<int> Count(IReadOnlyList<string> libraryIds)
        {
            return Count(Array.Empty<Filter>(), libraryIds);
        }

        public async Task<int> Count(Filter[] filters, IReadOnlyList<string> libraryIds)
        {
            throw new NotImplementedException();
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
{libraryIds.AddLibraryIdFilter("m")}";

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

        public void UpsertShows(IEnumerable<SqlShow> shows)
        {
            throw new NotImplementedException();
        }

        public void UpsertShow(SqlShow show)
        {
            throw new NotImplementedException();
        }

        public void InsertSeasons(IEnumerable<SqlSeason> seasons)
        {
            throw new NotImplementedException();
        }

        public void InsertEpisodes(IEnumerable<SqlEpisode> episodes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SqlShow> GetAllShows(IReadOnlyList<string> libraryIds, bool includeSeasons, bool includeEpisodes)
        {
            throw new NotImplementedException();
        }

        public SqlShow GetShowById(string showId, bool includeEpisodes)
        {
            throw new NotImplementedException();
        }

        public Task<SqlSeason> GetSeasonById(string id)
        {
            return _context.Seasons.FirstOrDefaultAsync(x => x.Id == id);
        }

        public IEnumerable<SqlEpisode> GetAllEpisodesForShow(string showId)
        {
            return _context.Shows
                .Include(x => x.Seasons)
                .ThenInclude(x => x.Episodes)
                .Where(x => x.Id == showId)
                .SelectMany(x => x.Seasons)
                .SelectMany(x => x.Episodes);
        }

        public SqlShow GetShowById(string showId)
        {
            throw new NotImplementedException();
        }

        public void RemoveShowsThatAreNotUpdated(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public void AddEpisode(SqlEpisode episode)
        {
            throw new NotImplementedException();
        }

        public void RemoveShows()
        {
            _context.Shows.RemoveRange(_context.Shows);
        }

        public Dictionary<SqlShow, int> GetShowsWithMostEpisodes(IReadOnlyList<string> libraryIds, int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SqlShow> GetShowPage(int skip, int take, string sort, Filter[] filters, List<string> libraryIds)
        {
            throw new NotImplementedException();
        }
    }
}
