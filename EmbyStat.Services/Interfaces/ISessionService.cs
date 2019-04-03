using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Services.Interfaces
{
    public interface ISessionService
    {
        List<string> GetMediaIdsForUser(string id, PlayType type);
        IEnumerable<Play> GetPlaysPageForUser(string id, int page, int size);
        int GetPlayCountForUser(string id);
        IEnumerable<PlayState> GetPlayStatesForUser(string id);
    }
}
