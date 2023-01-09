using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Query;
using EmbyStat.Core.MediaHelpers.Interfaces;

namespace EmbyStat.Core.Movies.Interfaces;

public interface IMovieRepository : IMediaRepository
{
    Task<Movie?> GetById(string id);
    Task UpsertRange(IEnumerable<Movie> movies);
    IEnumerable<Movie> GetAll();
    IEnumerable<Movie> GetAllWithImdbId();
    Task<long?> GetTotalRuntime();
    Task<long> GetPlayedRuntime();
    Task<List<Movie>> GetShortestMovie(long toShortMovieTicks, int count);
    Task<List<Movie>> GetLongestMovie(int count);
    Task<double> GetTotalDiskSpace();
    IEnumerable<Movie> GetToShortMovieList(int toShortMovieMinutes);
    IEnumerable<Movie> GetMoviesWithoutImdbId();
    IEnumerable<Movie> GetMoviesWithoutPrimaryImage();
    Task<IEnumerable<Movie>> GetMoviePage(int skip, int take, string sortField, string sortOrder, IEnumerable<Filter> filters);
    IEnumerable<LabelValuePair> CalculateSubtitleFilterValues();
    IEnumerable<LabelValuePair> CalculateContainerFilterValues();
    IEnumerable<LabelValuePair> CalculateGenreFilterValues();
    IEnumerable<LabelValuePair> CalculateCodecFilterValues();
    IEnumerable<LabelValuePair> CalculateVideoRangeFilterValues();
    Task DeleteAll();
    Task RemoveUnwantedMovies(IEnumerable<string> libraryIds);
    Task<int> GetTotalWatchedMovieCount();
    Task<Dictionary<Movie, int>> GetMostWatchedMovies(int count);
    Task<IEnumerable<BarValue<string,int>>> GetWatchedPerHourOfDayChartValues();
    Task<IEnumerable<BarValue<string, int>>> GetWatchedPerDayOfWeekChartValues();
    Task<int> GetCurrentWatchingCount();
    Task<List<Movie>> GetLatestAddedMovie(int count);
}