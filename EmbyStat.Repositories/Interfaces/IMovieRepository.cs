using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Repositories.Interfaces.Helpers;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Movies;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository : IMediaRepository
    {
        SqlMovie GetById(string id);
        void RemoveAll();
        Task UpsertRange(IEnumerable<SqlMovie> movies);
        IEnumerable<SqlMovie> GetAll();
        IEnumerable<SqlMovie> GetAllWithImdbId();
        long? GetTotalRuntime();
        IEnumerable<SqlMovie> GetShortestMovie(long toShortMovieTicks, int count);
        IEnumerable<SqlMovie> GetLongestMovie(int count);
        double GetTotalDiskSpace();
        IEnumerable<SqlMovie> GetToShortMovieList(int toShortMovieMinutes);
        IEnumerable<SqlMovie> GetMoviesWithoutImdbId();
        IEnumerable<SqlMovie> GetMoviesWithoutPrimaryImage();
        Task<IEnumerable<SqlMovie>> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters);
        IEnumerable<LabelValuePair> CalculateSubtitleFilterValues();
        IEnumerable<LabelValuePair> CalculateContainerFilterValues();
        IEnumerable<LabelValuePair> CalculateGenreFilterValues();
        IEnumerable<LabelValuePair> CalculateCodecFilterValues();
        IEnumerable<LabelValuePair> CalculateVideoRangeFilterValues();
        Task DeleteAll();
    }
}
