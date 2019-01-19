using System;
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
        MovieStats GetGeneralStatsForCollections(List<string> collectionIds);
        Task<PersonStats> GetPeopleStatsForCollections(List<string> collectionsIds);
        MovieGraphs GetGraphs(List<string> collectionIds);
        SuspiciousTables GetSuspiciousMovies(List<string> collectionIds);
        bool MovieTypeIsPresent();
    }
}
