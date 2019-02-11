using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;

namespace EmbyStat.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task ProcessSessions(List<Session> sessions)
        {
            foreach (var session in sessions)
            {
                var sessionExists = _eventRepository.DoesSessionsWithIdExists(session.Id);

                if (sessionExists && session.Plays.Count == 1)
                {
                    await _eventRepository.CreateOrAppendPlayLogs(session.Plays.Single());
                }
                else if(!sessionExists)
                {
                    await _eventRepository.CreateSession(session);
                }
                else
                {
                    //Not playing anything but session is alive
                }
            }
        }
    }
}
