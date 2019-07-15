using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using MediaBrowser.Model.Extensions;

namespace EmbyStat.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly LiteCollection<Session> _sessionCollection;
        private readonly LiteCollection<Play> _playCollection;

        public SessionRepository(IDbContext context)
        {
            _sessionCollection = context.GetContext().GetCollection<Session>();
            _playCollection = context.GetContext().GetCollection<Play>();
        }

        public bool DoesSessionExists(string sessionId)
        {
            return _sessionCollection.Exists(Query.EQ("_id", sessionId));
        }

        public void UpdateSession(Session session)
        {
            _sessionCollection.Update(session);
        }

        public Session GetSessionById(string sessionId)
        {
            return _sessionCollection.Include(x => x.Plays).FindById(sessionId);
        }

        public void CreateSession(Session session)
        {
            _sessionCollection.Insert(session);
        }

        public void InsertPlays(List<Play> sessionPlays)
        {
            _playCollection.InsertBulk(sessionPlays);
        }

        public IEnumerable<string> GetMediaIdsForUser(string userId, PlayType type)
        {
            return _playCollection.Find(Query.And(Query.EQ("UserId", userId), Query.EQ("Type", (int)type)))
                .DistinctBy(x => x.MediaId)
                .Select(x => x.MediaId);
        }

        public IEnumerable<Session> GetSessionsForUser(string userId)
        {
            return _sessionCollection.Find(Query.EQ("UserId", userId));
        }

        public int GetPlayCountForUser(string userId)
        {
            return _playCollection.Count(Query.EQ("UserId", userId));
        }
    }
}
