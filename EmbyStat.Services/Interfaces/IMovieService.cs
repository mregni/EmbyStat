using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Stats;

namespace EmbyStat.Services.Interfaces
{
    public interface IMovieService
    {
        List<Collection> GetMovieCollections();
        MovieStats GetGeneralStatsForCollections(List<string> collectionIds);
    }
}
