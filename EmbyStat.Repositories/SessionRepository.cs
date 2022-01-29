using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Repositories.Interfaces;

namespace EmbyStat.Repositories
{
    public class SessionRepository : BaseRepository, ISessionRepository
    {
        public SessionRepository(IDbContext context) : base(context)
        {
           
        }

        public bool DoesSessionExists(string sessionId)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Session>();
            return collection.Exists(x => x.Id == sessionId);
        }

        public void UpdateSession(Session session)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Session>();
            collection.Update(session);
        }

        public Session GetSessionById(string sessionId)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Session>();
            return collection.Include(x => x.Plays).FindById(sessionId);
        }

        public void CreateSession(Session session)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Session>();
            collection.Insert(session);
        }

        public void InsertPlays(List<Play> sessionPlays)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Play>();
            collection.InsertBulk(sessionPlays);
        }

        public IEnumerable<string> GetMediaIdsForUser(string userId, PlayType type)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Play>();
            return collection.Find(x => x.UserId == userId && x.Type == type)
                .DistinctBy(x => x.MediaId)
                .Select(x => x.MediaId);
        }

        public IEnumerable<Session> GetSessionsForUser(string userId)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Session>();
            return collection.Find(x => x.UserId == userId);
        }

        public int GetPlayCountForUser(string userId)
        {
            using var database = Context.CreateDatabaseContext();
            var collection = database.GetCollection<Play>();
            return collection.Count(x => x.UserId == userId);
        }
    }
}
