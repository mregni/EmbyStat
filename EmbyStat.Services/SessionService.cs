using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services;

public class SessionService : ISessionService
{
    //private readonly ISessionRepository _sessionRepository;

    public SessionService()
    {
    }

    public IEnumerable<string> GetMediaIdsForUser(string id, PlayType type)
    {
        throw new NotImplementedException();
        // return _sessionRepository.GetMediaIdsForUser(id, type);
    }

    public IEnumerable<Session> GetSessionsForUser(string id)
    {
        throw new NotImplementedException();
        //return _sessionRepository.GetSessionsForUser(id);
    }

    public int GetPlayCountForUser(string id)
    {
        throw new NotImplementedException();
        //return _sessionRepository.GetPlayCountForUser(id);
    }

    public void ProcessSessions(List<Session> sessions)
    {
        throw new NotImplementedException();
        // foreach (var session in sessions)
        // {
        //     var sessionExists = _sessionRepository.DoesSessionExists(session.Id);
        //
        //     if (sessionExists && session.Plays.Any())
        //     {
        //         var dbSession = _sessionRepository.GetSessionById(session.Id);
        //         dbSession.Plays.AddRange(session.Plays);
        //
        //         _sessionRepository.UpdateSession(dbSession);
        //         _sessionRepository.InsertPlays(session.Plays);
        //     }
        //     else if (!sessionExists)
        //     {
        //         _sessionRepository.CreateSession(session);
        //     }
        //     else
        //     {
        //         //Not playing anything but session is alive
        //     }
        // }
    }
}