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
        List<Collection> GetMovieCollections();
        MovieStats GetGeneralStatsForCollections(List<string> collectionIds);
        Task<PersonStats> GetPeopleStatsForCollections(List<string> collectionsIds);
        MovieGraphs GetGraphs(List<string> collectionIds);
        SuspiciousTables GetSuspiciousMovies(List<string> collectionIds);
        bool MovieTypeIsPresent();
    }
}
