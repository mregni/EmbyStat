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
        Task<MovieStatistics> GetMovieStatistics(List<string> collectionIds);
        bool TypeIsPresent();
    }
}
