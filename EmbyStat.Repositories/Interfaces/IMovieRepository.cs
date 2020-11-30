using EmbyStat.Common.Models.Entities;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Repositories.Interfaces.Helpers;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Query;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository : IMediaRepository<Movie>
    {
        void RemoveMovies();
        void UpsertRange(IEnumerable<Movie> movies);
        List<Movie> GetAll(IReadOnlyList<string> libraryIds);
        List<Movie> GetAllWithImdbId(IReadOnlyList<string> libraryIds);
        Movie GetMovieById(string id);
        long GetTotalRuntime(IReadOnlyList<string> libraryIds);
        IEnumerable<Movie> GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks, int count);
        IEnumerable<Movie> GetLongestMovie(IReadOnlyList<string> libraryIds, int count);
        double GetTotalDiskSize(IReadOnlyList<string> libraryIds);
        List<Movie> GetToShortMovieList(IReadOnlyList<string> libraryIds, int toShortMovieMinutes);
        List<Movie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds);
        List<Movie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds);
        IEnumerable<Movie> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters, List<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateSubtitleFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateContainerFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateGenreFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateCollectionFilterValues();
        IEnumerable<LabelValuePair> CalculateCodecFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateVideoRangeFilterValues(IReadOnlyList<string> libraryIds);
    }
}
