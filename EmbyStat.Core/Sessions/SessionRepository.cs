using Dapper;
using EmbyStat.Common;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.Sessions.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Core.Sessions;

public class SessionRepository : ISessionRepository
{
    private readonly EsDbContext _context;
    private readonly ISqliteBootstrap _sqliteBootstrap;

    public SessionRepository(EsDbContext context, ISqliteBootstrap sqliteBootstrap)
    {
        _context = context;
        _sqliteBootstrap = sqliteBootstrap;
    }

    public async Task UpsertRange(IEnumerable<Session> sessions)
    {
        var query = @$"
REPLACE INTO {Constants.Tables.Sessions} 
    (Id,ServerId,UserId,Client,DeviceId,ApplicationVersion,AppIconUrl,RemoteEndPoint,Protocol,LastActivityDate) 
VALUES (@Id,@ServerId,@UserId,@Client,@DeviceId,@ApplicationVersion,@AppIconUrl,@RemoteEndPoint,@Protocol,@LastActivityDate)";
        await _sqliteBootstrap.ExecuteQuery(query, sessions);
    }

    public MediaPlay? GetActiveMediaPlay(string sessionId, string userId, string mediaId)
    {
        return _context.MediaPlays
            .AsNoTracking()
            .FirstOrDefault(x =>
                x.SessionId == sessionId &&
                x.UserId == userId &&
                x.MediaId == mediaId &&
                x.Stop == null);
    }

    public void InsertMediaPlay(MediaPlay play)
    {
        _context.MediaPlays.Add(play);
        _context.SaveChanges();
    }

    public void UpdateMediaPlay(MediaPlay play)
    {
        _context.MediaPlays.Update(play);
        _context.SaveChanges();
    }

    public void UpdateMediaPlays(IEnumerable<MediaPlay> plays)
    {
        _context.MediaPlays.UpdateRange(plays);
        _context.SaveChanges();
    }

    public IEnumerable<MediaPlay> GetRunningMediaPlays()
    {
        return _context.MediaPlays
            .AsNoTracking()
            .Where(x => x.Stop == null);
    }
}