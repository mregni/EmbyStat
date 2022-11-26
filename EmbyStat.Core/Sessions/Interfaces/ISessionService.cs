using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Sessions;

namespace EmbyStat.Core.Sessions.Interfaces;

public interface ISessionService
{
    void ProcessSessions(IEnumerable<WebSocketSession> sessions);
}