using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Users;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories;

public class SqlMediaServerRepository : IMediaServerRepository
{
    private readonly SqlLiteDbContext _context;

    public SqlMediaServerRepository(SqlLiteDbContext context)
    {
        _context = context;
    }


    #region MediaServer Status
    public EmbyStatus GetEmbyStatus()
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
    public Task<List<SqlPluginInfo>> GetAllPlugins()
    {
        return _context.Plugins.ToListAsync();
    }

    public async Task InsertPlugins(IEnumerable<SqlPluginInfo> plugins)
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
    
    public Task<SqlServerInfo> GetServerInfo()
    {
        return _context.ServerInfo.SingleOrDefaultAsync();
    }

    public async Task UpsertServerInfo(SqlServerInfo entity)
    {
        var info = await _context.ServerInfo.AsNoTracking().SingleOrDefaultAsync();
        if (info == null)
        {
            await _context.ServerInfo.AddAsync(entity);
        }
        else
        {
            _context.ServerInfo.Update(entity);
        }
        
        await _context.SaveChangesAsync();
    }
    
    #endregion

    #region MediaServer Users
    public async Task UpsertUsers(IEnumerable<SqlUser> users)
    {
        _context.Users.RemoveRange(_context.Users);
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }

    public Task<List<SqlUser>> GetAllUsers()
    {
        return _context.Users
            .Include(x => x.Configuration)
            .Include(x => x.Policy)
            .ToListAsync();
    }

    public Task<List<SqlUser>> GetAllAdministrators()
    {
        return _context.Users
            .Include(x => x.Configuration)
            .Include(x => x.Policy)
            .Where(x => x.Policy.IsAdministrator)
            .ToListAsync();
    }

    public EmbyUser GetUserById(string id)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Devices
    public Task<List<SqlDevice>> GetAllDevices()
    {
        return _context.Devices.ToListAsync();
    }

    public Task<List<SqlDevice>> GetDeviceById(IEnumerable<string> ids)
    {
        throw new System.NotImplementedException();
    }

    public async Task UpsertDevices(IEnumerable<SqlDevice> devices)
    {
        _context.Devices.RemoveRange(_context.Devices);
        await _context.Devices.AddRangeAsync(devices);
        await _context.SaveChangesAsync();
    }
    #endregion
}