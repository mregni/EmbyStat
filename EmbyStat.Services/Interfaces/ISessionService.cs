using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Services.Interfaces
{
    public interface ISessionService
    {
        List<string> GetMediaIdsForUser(string id, PlayType type);
        List<Play> GetLastWatchedMediaForUser(string id, int count);
    }
}
