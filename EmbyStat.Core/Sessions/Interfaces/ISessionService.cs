using EmbyStat.Common.Models.Sessions;

namespace EmbyStat.Core.Sessions.Interfaces;

public interface ISessionService
{
    void ProcessSessions(IEnumerable<WebSocketSession> sessions);
}