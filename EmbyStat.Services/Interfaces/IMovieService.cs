using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Services.Models;
using EmbyStat.Services.Models.Graph;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IMovieService
    {
        IEnumerable<Collection> GetMovieCollections();
        MovieStats GetGeneralStatsForCollections(List<Guid> collectionIds);
        Task<PersonStats> GetPeopleStatsForCollections(List<Guid> collectionsIds);
        MovieGraphs GetGraphs(List<Guid> collectionIds);
        SuspiciousTables GetSuspiciousMovies(List<Guid> collectionIds);
        bool MovieTypeIsPresent();
    }
}
