using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IEventRepository
    {
        bool DoesSessionsWithIdExists(string sessionId);
        Task CreateOrAppendPlayLogs(Play play);
        Task CreateSession(Session session);
    }
}
