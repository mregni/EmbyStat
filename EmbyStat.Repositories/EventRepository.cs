using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool DoesSessionsWithIdExists(string sessionId)
        {
            return _context.Sessions.Any(x => x.Id == sessionId);
        }

        public async Task CreateOrAppendPlayLogs(Play play)
        {
            var playObj = _context.Plays.SingleOrDefault(x => x.MediaId == play.MediaId);
            if (playObj == null)
            {
                _context.Plays.Add(play);
            }
            else
            {
                playObj.PlayStates.Add(play.PlayStates.Single());
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateSession(Session session)
        {
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
        }
    }
}
