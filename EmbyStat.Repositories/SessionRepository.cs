using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Repositories.Interfaces;
using LiteDB;
using MediaBrowser.Model.Extensions;

namespace EmbyStat.Repositories
{
    public class SessionRepository : BaseRepository, ISessionRepository
    {
        private readonly LiteCollection<Session> _sessionCollection;
        private readonly LiteCollection<Play> _playCollection;

        public SessionRepository(IDbContext context) : base(context)
        {
            _sessionCollection = context.GetContext().GetCollection<Session>();
            _playCollection = context.GetContext().GetCollection<Play>();
        }

        public bool DoesSessionExists(string sessionId)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Session>();
                    return collection.Exists(Query.EQ("_id", sessionId));
                }
            });
        }

        public void UpdateSession(Session session)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Session>();
                    collection.Update(session);
                }
            });
        }

        public Session GetSessionById(string sessionId)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Session>();
                    return collection.Include(x => x.Plays).FindById(sessionId);
                }
            });
        }

        public void CreateSession(Session session)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Session>();
                    collection.Insert(session);
                }
            });
        }

        public void InsertPlays(List<Play> sessionPlays)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Play>();
                    collection.InsertBulk(sessionPlays);
                }
            });
        }

        public IEnumerable<string> GetMediaIdsForUser(string userId, PlayType type)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Play>();
                    return collection.Find(Query.And(Query.EQ("UserId", userId), Query.EQ("Type", (int)type)))
                        .DistinctBy(x => x.MediaId)
                        .Select(x => x.MediaId);
                }
            });
        }

        public IEnumerable<Session> GetSessionsForUser(string userId)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Session>();
                    return collection.Find(Query.EQ("UserId", userId));
                }
            });
        }

        public int GetPlayCountForUser(string userId)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Play>();
                    return collection.Count(Query.EQ("UserId", userId));
                }
            });
        }
    }
}
