using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        bool DoesSessionExists(string sessionId);
        void UpsertPlayLogs(Session session);
        void CreateSession(Session session);
        IEnumerable<string> GetMediaIdsForUser(string userId, PlayType type);
        IEnumerable<Session> GetSessionsForUser(string userId);
        int GetPlayCountForUser(string UserId);
    }
}
