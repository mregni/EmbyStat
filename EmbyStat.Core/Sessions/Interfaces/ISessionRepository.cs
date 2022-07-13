using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Core.Sessions.Interfaces;

public interface ISessionRepository
{
    IEnumerable<string> GetMediaIdsForUser(string userId, MediaType type);
    IEnumerable<Session> GetSessionsForUser(string userId);
    int GetPlayCountForUser(string userId);
    Session GetSessionById(string sessionId);
}