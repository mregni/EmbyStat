using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Core.Sessions.Interfaces;

public interface ISessionRepository
{
    Task UpsertRange(IEnumerable<Session> sessions);
    MediaPlay? GetActiveMediaPlay(string sessionId, string userId, string mediaId);
    void InsertMediaPlay(MediaPlay play);
    void UpdateMediaPlay(MediaPlay play);
    void UpdateMediaPlays(IEnumerable<MediaPlay> plays);
    IEnumerable<MediaPlay> GetRunningMediaPlays();
}