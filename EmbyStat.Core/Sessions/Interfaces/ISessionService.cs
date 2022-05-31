using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Core.Sessions.Interfaces;

public interface ISessionService
{
    IEnumerable<string> GetMediaIdsForUser(string id, PlayType type);
    IEnumerable<Session> GetSessionsForUser(string id);
    int GetPlayCountForUser(string id);
    void ProcessSessions(List<Session> sessions);
}