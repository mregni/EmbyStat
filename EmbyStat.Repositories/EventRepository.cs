using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class EventRepository : IEventRepository
    {
        public bool DoesSessionsWithIdExists(string sessionId)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Sessions.Any(x => x.Id == sessionId);
            }
        }

        public async Task CreateOrAppendPlayLogs(Play play)
        {
            using (var context = new ApplicationDbContext())
            {
                var playObj = context.Plays.SingleOrDefault(x => x.MediaId == play.MediaId);
                if (playObj == null)
                {
                    context.Plays.Add(play);
                }
                else
                {
                    playObj.PlayStates.Add(play.PlayStates.Single());
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task CreateSession(Session session)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Sessions.Add(session);
                await context.SaveChangesAsync();
            }
        }
    }
}
