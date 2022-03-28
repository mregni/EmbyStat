using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories;

public class SqlMediaServerRepository : IMediaServerRepository
{
    private readonly DbContext _context;

    public SqlMediaServerRepository(DbContext context)
    {
        _context = context;
    }

    #region MediaServer Status
    public MediaServerStatus GetEmbyStatus()
    {
        throw new System.NotImplementedException();
    }

    public void IncreaseMissedPings()
    {
        throw new System.NotImplementedException();
    }

    public void ResetMissedPings()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveAllMediaServerData()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region MediaServer Plugins
    public Task<List<PluginInfo>> GetAllPlugins()
    {
        return _context.Plugins.ToListAsync();
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
        return _context.MediaServerInfo.SingleOrDefaultAsync();
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
        _context.MediaServerUsers.RemoveRange(_context.MediaServerUsers);
        await _context.MediaServerUsers.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }

    public Task<List<MediaServerUser>> GetAllUsers()
    {
        return _context.MediaServerUsers
            .Include(x => x.Configuration)
            .Include(x => x.Policy)
            .ToListAsync();
    }

    public Task<List<MediaServerUser>> GetAllAdministrators()
    {
        return _context.MediaServerUsers
            .Include(x => x.Configuration)
            .Include(x => x.Policy)
            .Where(x => x.Policy.IsAdministrator)
            .ToListAsync();
    }

    public EmbyUser GetUserById(string id)
    {
        throw new System.NotImplementedException();
    }

    public async Task DeleteAllUsers()
    {
        _context.MediaServerUsers.RemoveRange(_context.MediaServerUsers);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region Devices
    public Task<List<Device>> GetAllDevices()
    {
        return _context.Devices.ToListAsync();
    }

    public Task<List<Device>> GetDeviceById(IEnumerable<string> ids)
    {
        throw new System.NotImplementedException();
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
        return _context.Libraries.ToListAsync();
    }

    public Task<List<Library>> GetAllLibraries(LibraryType type)
    {
        return _context.Libraries.Where(x => x.Type == type).ToListAsync();
    }
    
    public Task<List<Library>> GetAllLibraries(LibraryType type, bool synced)
    {
        return _context.Libraries
            .Where(x => x.Type == type && x.Sync == synced)
            .ToListAsync();
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

    #endregion
}