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

        public void UpsertPlayLogs(Session session)
        {
            var dbSession = _sessionCollection.FindById(session.Id);
            var dbPlay = _playCollection.FindById(session.Plays.Single().Id);

            if (dbPlay == null)
            {
                dbSession.Plays.Add(session.Plays.Single());
            }
            else
            {
                dbPlay.PlayStates.Add(session.Plays.Single().PlayStates.Single());
            }

            _sessionCollection.Update(dbSession);
        }

        public void CreateSession(Session session)
        {
            _sessionCollection.Insert(session);
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
