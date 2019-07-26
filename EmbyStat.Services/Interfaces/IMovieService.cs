using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IMovieService
    {
        IEnumerable<Collection> GetMovieCollections();
        Task<MovieStatistics> GetMovieStatisticsAsync(List<string> collectionIds);
        Task<MovieStatistics> CalculateMovieStatistics(List<string> collectionIds);
        bool TypeIsPresent();
    }
}
