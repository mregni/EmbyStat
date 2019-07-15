using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        bool DoesSessionExists(string sessionId);
        void UpdateSession(Session session);
        void CreateSession(Session session);
        void InsertPlays(List<Play> sessionPlays);
        IEnumerable<string> GetMediaIdsForUser(string userId, PlayType type);
        IEnumerable<Session> GetSessionsForUser(string userId);
        int GetPlayCountForUser(string UserId);
        Session GetSessionById(string sessionId);
    }
}
