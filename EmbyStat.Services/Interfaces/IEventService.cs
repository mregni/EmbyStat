using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Services.Interfaces
{
    public interface IEventService
    {
        Task ProcessSessions(List<Session> sessions);
    }
}
