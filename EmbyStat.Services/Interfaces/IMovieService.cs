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
        Task<MovieStats> GetGeneralStatsForCollections(List<string> collectionIds);
        Task<PersonStats> GetPeopleStatsForCollections(List<string> collectionsIds);
        Task<MovieCharts> GetCharts(List<string> collectionIds);
        Task<SuspiciousTables> GetSuspiciousMovies(List<string> collectionIds);
        bool MovieTypeIsPresent();
    }
}
