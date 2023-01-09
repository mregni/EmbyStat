using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Query;

namespace EmbyStat.Core.MediaHelpers.Interfaces;

public interface IMediaRepository
{
    Task<IEnumerable<Media>> GetNewestPremieredMedia(int count);
    Task<IEnumerable<Media>> GetOldestPremieredMedia(int count);
    Task<IEnumerable<Extra>> GetHighestRatedMedia(int count);
    Task<IEnumerable<Extra>> GetLowestRatedMedia(int count);

    #region Charts
    Task<Dictionary<string, int>> GetGenreChartValues();
    Task<List<decimal?>> GetCommunityRatings();
    Task<List<DateTime?>> GetPremiereYears();
    Task<Dictionary<string, int>> GetOfficialRatingChartValues();
    #endregion

    Task<int> Count();
    Task<int> Count(Filter[] filters);
    bool Any();
    Task<int> GetGenreCount();
    Task<int> GetPeopleCount(PersonType type);
}