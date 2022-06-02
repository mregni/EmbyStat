using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Common.Models.MediaServer;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.MediaServers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmbyStat.Core.MediaServers;

public class MediaServerRepository : IMediaServerRepository
{
    private readonly EsDbContext _context;
    private readonly ISqliteBootstrap _sqliteBootstrap;
    private readonly ILogger<MediaServerRepository> _logger;

    public MediaServerRepository(EsDbContext context, ISqliteBootstrap sqliteBootstrap,
        ILogger<MediaServerRepository> logger)
    {
        _context = context;
        _sqliteBootstrap = sqliteBootstrap;
        _logger = logger;
    }

    #region MediaServer Status

    public Task<MediaServerStatus> GetEmbyStatus()
    {
        return _context.MediaServerStatus
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task IncreaseMissedPings()
    {
        var status = await _context.MediaServerStatus.FirstOrDefaultAsync();
        if (status != null)
        {
            status.MissedPings += 1;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ResetMissedPings()
    {
        var status = await _context.MediaServerStatus.FirstOrDefaultAsync();
        if (status != null)
        {
            status.MissedPings = 0;
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region MediaServer Plugins

    public Task<List<PluginInfo>> GetAllPlugins()
    {
        return _context.Plugins.AsNoTracking().ToListAsync();
    }

    public async Task InsertPlugins(IEnumerable<PluginInfo> plugins)
    {
        await _context.AddRangeAsync(plugins);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAllPlugins()
    {
        _context.Plugins.RemoveRange(_context.Plugins);
        return _context.SaveChangesAsync();
    }

    #endregion

    #region MediaServer Server Info

    public Task<MediaServerInfo> GetServerInfo()
    {
        return _context.MediaServerInfo.AsNoTracking().SingleOrDefaultAsync();
    }

    public async Task DeleteAndInsertServerInfo(MediaServerInfo entity)
    {
        _context.MediaServerInfo.RemoveRange(_context.MediaServerInfo);
        await _context.MediaServerInfo.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteServerInfo()
    {
        _context.MediaServerInfo.RemoveRange(_context.MediaServerInfo);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region MediaServer Users

    public async Task DeleteAndInsertUsers(IEnumerable<MediaServerUser> users)
    {
        var query = MediaServerExtensions.UserInsertQuery;
        _logger.LogDebug(query);
        
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        
        await using var transaction = connection.BeginTransaction();
        await connection.ExecuteAsync(query, users, transaction);
        await transaction.CommitAsync();

        var usersToDelete = await _context.MediaServerUsers
            .Where(x => !users.Select(u => u.Id).Contains(x.Id))
            .ToListAsync();
        
        _context.MediaServerUsers.RemoveRange(usersToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<MediaServerUserRow>> GetUserPage(int skip, int take, string sortField, string sortOrder)
    {
        var query = MediaServerExtensions.GenerateUserPageQuery(skip, take, sortField, sortOrder);
        _logger.LogDebug(query);
        
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        return await connection.QueryAsync<MediaServerUserRow>(query);
    }

    public Task<MediaServerUser[]> GetAllUsers()
    {
        return _context
            .MediaServerUsers
            .AsNoTracking()
            .ToArrayAsync();
    }

    public Task<MediaServerUser[]> GetAllAdministrators()
    {
        return _context.MediaServerUsers
            .AsNoTracking()
            .Where(x => x.IsAdministrator)
            .ToArrayAsync();
    }

    public Task<MediaServerUser> GetUserById(string id)
    {
        return _context.MediaServerUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task DeleteAllUsers()
    {
        _context.MediaServerUsers.RemoveRange(_context.MediaServerUsers);
        await _context.SaveChangesAsync();
    }

    public async Task InsertOrUpdateUserViews(List<MediaServerUserView> views)
    {
        var query = $@"
INSERT INTO {Constants.Tables.MediaServerUserViews} (UserId, MediaId, LastPlayedDate, MediaType, PlayCount)
VALUES (@UserId, @MediaId, @LastPlayedDate, @MediaType, @PlayCount)
ON CONFLICT (UserId, MediaId) DO
UPDATE SET LastPlayedDate=excluded.LastPlayedDate, PlayCount=excluded.PlayCount;";

        _logger.LogDebug(query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        
        await using var transaction = connection.BeginTransaction();
        await connection.ExecuteAsync(query, views, transaction);
        await transaction.CommitAsync();
    }

    public Task<int> GetUserCount()
    {
        return _context.MediaServerUsers.CountAsync();
    }

    public int GetUserViewsForType(string type)
    {
        var list = _context.MediaServerUserViews
            .AsNoTracking()
            .Where(x => x.MediaType == type)
            .AsEnumerable();

        return list.DistinctBy(x => x.MediaId).Count();
    }

    public async Task<Dictionary<Show, int>> GetMostWatchedShows(int count)
    {
        var query = $@"
SELECT s.*, COUNT(*) AS ViewCount
FROM {Constants.Tables.MediaServerUserViews} AS vi
INNER JOIN {Constants.Tables.Episodes} AS e ON (vi.MediaId = e.Id)
INNER JOIN {Constants.Tables.Seasons} AS se ON (se.Id = e.SeasonId)
INNER JOIN {Constants.Tables.Shows} AS s ON (s.Id = se.ShowId)
WHERE vi.MediaType = 'Episode'
GROUP BY s.Id
ORDER BY ViewCount DESC
LIMIT {count}";
        
        _logger.LogDebug(query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        var result = await connection
            .QueryAsync<Show, long, KeyValuePair<Show, int>>(query,
            (s, c) => new KeyValuePair<Show, int>(s, Convert.ToInt32(c)),
            splitOn: "ViewCount");

        return result.ToDictionary(x => x.Key, x => x.Value);
    }
    
    public async Task<Dictionary<Movie, int>> GetMostWatchedMovies(int count)
    {
        var query = $@"
SELECT m.*, COUNT(*) AS ViewCount
FROM {Constants.Tables.MediaServerUserViews} AS vi
INNER JOIN {Constants.Tables.Movies} AS m ON (vi.MediaId = m.Id)
WHERE vi.MediaType = 'Movie'
GROUP BY m.Id
ORDER BY ViewCount DESC
LIMIT {count}";
        
        _logger.LogDebug(query);
        await using var connection = _sqliteBootstrap.CreateConnection();
        await connection.OpenAsync();
        var result = await connection
            .QueryAsync<Movie, long, KeyValuePair<Movie, int>>(query,
                (m, c) => new KeyValuePair<Movie, int>(m, Convert.ToInt32(c)),
                splitOn: "ViewCount");

        return result.ToDictionary(x => x.Key, x => x.Value);
    }

    #endregion

    #region Devices

    public Task<List<Device>> GetAllDevices()
    {
        return _context.Devices.AsNoTracking().ToListAsync();
    }

    public async Task DeleteAndInsertDevices(IEnumerable<Device> devices)
    {
        _context.Devices.RemoveRange(_context.Devices);
        await _context.Devices.AddRangeAsync(devices);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllDevices()
    {
        _context.Devices.RemoveRange(_context.Devices);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Libraries

    public Task<List<Library>> GetAllLibraries()
    {
        return _context.Libraries.AsNoTracking().ToListAsync();
    }

    public Task<List<Library>> GetAllLibraries(LibraryType type)
    {
        return _context.Libraries
            .AsNoTracking()
            .Where(x => x.Type == type).ToListAsync();
    }

    public Task<List<Library>> GetAllLibraries(LibraryType type, bool synced)
    {
        return _context.Libraries
            .Where(x => x.Type == type && x.Sync == synced)
            .ToListAsync();
    }

    public async Task SetLibraryAsSynced(string[] libraryIds, LibraryType type)
    {
        var libraries = await _context.Libraries
            .Where(x => x.Type == type)
            .ToListAsync();
        
        foreach (var library in libraries)
        {
            library.Sync = libraryIds.Contains(library.Id);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAndInsertLibraries(Library[] libraries)
    {
        _context.Libraries.RemoveRange(_context.Libraries);
        await _context.Libraries.AddRangeAsync(libraries);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllLibraries()
    {
        _context.Libraries.RemoveRange(_context.Libraries);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLibrarySyncDate(string libraryId, DateTime date)
    {
        var library = await _context.Libraries
            .Where(x => x.Id == libraryId)
            .FirstOrDefaultAsync();

        if (library != null)
        {
            library.LastSynced = date;
            await _context.SaveChangesAsync();
        }
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    #endregion
}