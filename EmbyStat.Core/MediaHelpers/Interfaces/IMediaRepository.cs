using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Query;

namespace EmbyStat.Core.MediaHelpers.Interfaces;

public interface IMediaRepository
{
    IEnumerable<Common.Models.Entities.Helpers.Media> GetLatestAddedMedia(int count);
    Task<IEnumerable<Common.Models.Entities.Helpers.Media>> GetNewestPremieredMedia(int count);
    Task<IEnumerable<Common.Models.Entities.Helpers.Media>> GetOldestPremieredMedia(int count);
    Task<IEnumerable<Extra>> GetHighestRatedMedia(int count);
    Task<IEnumerable<Extra>> GetLowestRatedMedia(int count);

    #region Charts
    Task<Dictionary<string, int>> GetGenreChartValues();
    IEnumerable<decimal?> GetCommunityRatings();
    IEnumerable<DateTime?> GetPremiereYears();
    Task<Dictionary<string, int>> GetOfficialRatingChartValues();
    #endregion

    Task<int> Count();
    Task<int> Count(Filter[] filters);
    bool Any();
    Task<int> GetGenreCount();
    int GetPeopleCount(PersonType type);
}