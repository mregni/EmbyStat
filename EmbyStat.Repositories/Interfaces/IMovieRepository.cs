using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Repositories.Interfaces.Helpers;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Movies;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository : IMediaRepository
    {
        SqlMovie GetById(string id);
        void RemoveAll();
        Task UpsertRange(IEnumerable<SqlMovie> movies);
        IEnumerable<SqlMovie> GetAll(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetAllWithImdbId(IReadOnlyList<string> libraryIds);
        long? GetTotalRuntime(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks, int count);
        IEnumerable<SqlMovie> GetLongestMovie(IReadOnlyList<string> libraryIds, int count);
        double GetTotalDiskSpace(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetToShortMovieList(IReadOnlyList<string> libraryIds, int toShortMovieMinutes);
        IEnumerable<SqlMovie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds);
        Task<IEnumerable<SqlMovie>> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters, List<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateSubtitleFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateContainerFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateGenreFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateCollectionFilterValues();
        IEnumerable<LabelValuePair> CalculateCodecFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateVideoRangeFilterValues(IReadOnlyList<string> libraryIds);
        Task DeleteAll();
    }
}
