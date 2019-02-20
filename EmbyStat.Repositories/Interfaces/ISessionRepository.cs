using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;

namespace EmbyStat.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        List<string> GetMediaIdsForUser(string id, PlayType type);
        List<Play> GetPlaysForUser(string id);
    }
}
