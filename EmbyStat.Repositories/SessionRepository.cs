using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<string> GetMediaIdsForUser(string id, PlayType type)
        {
            return _context.Sessions
                .Where(x => x.UserId == id)
                .Include(x => x.Plays)
                .SelectMany(x => x.Plays)
                .Where(x => x.Type == type)
                .Select(x => x.MediaId)
                .ToList();
        }

        public List<Play> GetPlaysForUser(string id)
        {
            return _context.Sessions
                .Where(x => x.UserId == id)
                .Include(x => x.Plays)
                .SelectMany(x => x.Plays)
                .Include(x => x.PlayStates)
                .Include(x => x.Session)
                .ToList();
        }
    }
}
