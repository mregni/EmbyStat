using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Services.Models;
using EmbyStat.Services.Models.Graph;
using EmbyStat.Services.Models.Movie;

namespace EmbyStat.Services.Interfaces
{
    public interface IMovieService
    {
        List<Collection> GetMovieCollections();
        MovieStats GetGeneralStatsForCollections(List<string> collectionIds);
        Task<MoviePersonStats> GetPeopleStatsForCollections(List<string> collectionsIds);
        List<MovieDuplicate> GetDuplicates(List<string> collectionIds);
        List<Graph> GetGraphs(List<string> collectionIds);
    }
}
