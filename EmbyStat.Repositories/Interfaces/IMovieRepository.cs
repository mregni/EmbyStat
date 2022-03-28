using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Repositories.Interfaces.Helpers;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Entities.Movies;
using EmbyStat.Common.Models.Query;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository : IMediaRepository
    {
        Movie GetById(string id);
        void RemoveAll();
        Task UpsertRange(IEnumerable<Movie> movies);
        IEnumerable<Movie> GetAll();
        IEnumerable<Movie> GetAllWithImdbId();
        long? GetTotalRuntime();
        IEnumerable<Movie> GetShortestMovie(long toShortMovieTicks, int count);
        IEnumerable<Movie> GetLongestMovie(int count);
        double GetTotalDiskSpace();
        IEnumerable<Movie> GetToShortMovieList(int toShortMovieMinutes);
        IEnumerable<Movie> GetMoviesWithoutImdbId();
        IEnumerable<Movie> GetMoviesWithoutPrimaryImage();
        Task<IEnumerable<Movie>> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters);
        IEnumerable<LabelValuePair> CalculateSubtitleFilterValues();
        IEnumerable<LabelValuePair> CalculateContainerFilterValues();
        IEnumerable<LabelValuePair> CalculateGenreFilterValues();
        IEnumerable<LabelValuePair> CalculateCodecFilterValues();
        IEnumerable<LabelValuePair> CalculateVideoRangeFilterValues();
        Task DeleteAll();
    }
}
