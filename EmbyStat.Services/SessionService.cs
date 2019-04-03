using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using SQLitePCL;

namespace EmbyStat.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public List<string> GetMediaIdsForUser(string id, PlayType type)
        {
            return _sessionRepository.GetMediaIdsForUser(id, type);
        }

        public IEnumerable<Play> GetPlaysPageForUser(string id, int page, int size)
        {
            var plays = _sessionRepository.GetPlaysForUser(id);
            var cleanedPlays = CleanPlayList(plays);

            return cleanedPlays
                .Select(x => x.PlayStates.First())
                .OrderByDescending(x => x.TimeLogged)
                .Select(x => x.Play)
                .Skip(page * size)
                .Take(size);
        }

        public IEnumerable<PlayState> GetPlayStatesForUser(string id)
        {
            return _sessionRepository.GetPlayStatesForUser(id);
        }

        public int GetPlayCountForUser(string id)
        {
            var plays = _sessionRepository.GetPlaysForUser(id);
            return CleanPlayList(plays).Count();
        }

        /// <summary>
        /// When 2 states in the same play are logged more then 1 hour time in between we calculate this as a new play entry.
        /// If not, we will presume that the previous state is connected with the current one.
        /// </summary>
        /// <param name="plays">List of Play that needs to be cleaned</param>
        /// <returns>Clean Play list</returns>
        private IEnumerable<Play> CleanPlayList(IEnumerable<Play> plays)
        {
            foreach (var play in plays)
            {
                PlayState prevState = null;
                foreach (var state in play.PlayStates)
                {
                    if (prevState == null || state.TimeLogged > prevState.TimeLogged.AddHours(1))
                    {
                        yield return play;
                    }

                    prevState = state;
                }
            }
        }
    }
}
