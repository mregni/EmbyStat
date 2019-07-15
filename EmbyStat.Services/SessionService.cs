using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public IEnumerable<string> GetMediaIdsForUser(string id, PlayType type)
        {
            return _sessionRepository.GetMediaIdsForUser(id, type);
        }

        public IEnumerable<Session> GetSessionsForUser(string id)
        {
            return _sessionRepository.GetSessionsForUser(id);
        }

        public int GetPlayCountForUser(string id)
        {
            return _sessionRepository.GetPlayCountForUser(id);
        }

        public void ProcessSessions(List<Session> sessions)
        {
            foreach (var session in sessions)
            {
                var sessionExists = _sessionRepository.DoesSessionExists(session.Id);

                if (sessionExists && session.Plays.Any())
                {
                    var dbSession = _sessionRepository.GetSessionById(session.Id);
                    dbSession.Plays.AddRange(session.Plays);

                    _sessionRepository.UpdateSession(dbSession);
                    _sessionRepository.InsertPlays(session.Plays);
                }
                else if (!sessionExists)
                {
                    _sessionRepository.CreateSession(session);
                }
                else
                {
                    //Not playing anything but session is alive
                }
            }
        }
    }
}
