using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Interfaces
{
    public interface IShowService
    {
        IEnumerable<Library> GetShowLibraries();
        Task<ShowStatistics> GetStatistics(List<string> libraryIds);
        List<ShowCollectionRow> GetCollectedRows(List<string> libraryIds);
        Task<ShowStatistics> CalculateShowStatistics(List<string> libraryIds);
        bool TypeIsPresent();
    }
}
