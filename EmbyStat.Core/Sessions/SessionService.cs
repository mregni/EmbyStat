using AutoMapper;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Sessions;
using EmbyStat.Core.Sessions.Interfaces;

namespace EmbyStat.Core.Sessions;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IMapper _mapper;

    public SessionService(ISessionRepository sessionRepository, IMapper mapper)
    {
        _sessionRepository = sessionRepository;
        _mapper = mapper;
    }

    public void ProcessSessions(IEnumerable<WebSocketSession> sessions)
    {
        var localSessions = _mapper.Map<IList<Session>>(sessions);
        var insertTask = _sessionRepository.UpsertRange(localSessions);
        insertTask.Wait();

        var plays = _mapper.Map<IList<MediaPlay>>(sessions.Where(x => x.NowPlayingItem != null));
        
        foreach (var play in plays)
        {
            var internalPlay = _sessionRepository.GetActiveMediaPlay(play.SessionId, play.UserId, play.MediaId);
            if (internalPlay == null)
            {
                play.Start = play.LastUpdate;
                play.StartPositionTicks = play.EndPositionTicks;
                _sessionRepository.InsertMediaPlay(play);
                continue;
            }

            play.Id = internalPlay.Id;
            play.Start = internalPlay.Start;
            play.StartPositionTicks = internalPlay.StartPositionTicks;
            _sessionRepository.UpdateMediaPlay(play);
        }

        StopRunningMediaPlays(plays);
    }

    private void StopRunningMediaPlays(IEnumerable<MediaPlay> plays)
    {
        var runningPlays = _sessionRepository.GetRunningMediaPlays();
        var stoppedPlays = runningPlays
            .Where(x => plays
                .All(y => !(y.SessionId == x.SessionId &&
                          y.UserId == x.UserId &&
                          y.MediaId == x.MediaId)))
            .ToList();
        
        stoppedPlays.ForEach(x =>
        {
            x.Stop = DateTime.Now;
            x.IsPaused = false;
        });
        _sessionRepository.UpdateMediaPlays(stoppedPlays);
    }
}