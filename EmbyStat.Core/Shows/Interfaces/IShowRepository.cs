using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
using EmbyStat.Core.MediaHelpers.Interfaces;

namespace EmbyStat.Core.Shows.Interfaces;

public interface IShowRepository : IMediaRepository
{
    #region Shows
    Task UpsertShows(IEnumerable<Show> shows);
    Task<IEnumerable<Show>> GetAllShowsWithEpisodes();
    Task<Show> GetShowByIdWithEpisodes(string showId);
    Task<Dictionary<Show, int>> GetShowsWithMostEpisodes(int count);
    Task<IEnumerable<Show>> GetShowPage(int skip, int take, string sortField, string sortOrder, IEnumerable<Filter> filters);
    Task<List<Show>> GetShowsWithMostDiskSpaceUsed(int count);
    IEnumerable<string> GetShowIdsThatFailedExternalSync(string libraryId);

    Task<int> CompleteCollectedCount();
    Task DeleteAll();
    Task RemoveUnwantedShows(IEnumerable<string> libraryIds);
    Task<Dictionary<Show, int>> GetMostWatchedShows(int count);
    #endregion

    #region Charts
    Task<Dictionary<string, int>> GetShowStatusCharValues();
    Task<IEnumerable<double>> GetCollectedRateChart();
    #endregion

    #region Episodes
    Task<int> GetEpisodeCount(LocationType locationType);
    Task<long> GetTotalRunTimeTicks();
    Task<double> GetTotalDiskSpaceUsed();
    Task<IEnumerable<BarValue<string, int>>> GetWatchedPerDayOfWeekChartValues();
    Task<IEnumerable<BarValue<string, int>>> GetWatchedPerHourOfDayChartValues();
    #endregion

    IEnumerable<LabelValuePair> CalculateGenreFilterValues();
    Task<int> GetTotalWatchedEpisodeCount();
    Task<long> GetPlayedRuntime();
    Task<int> GetCurrentWatchingCount();
    Task<List<Show>> GetLatestAddedShows(int count);
}