using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Services.Models.DataGrid;
using EmbyStat.Services.Models.Show;

namespace EmbyStat.Services.Interfaces
{
    public interface IShowService
    {
        IEnumerable<Library> GetShowLibraries();
        Task<ShowStatistics> GetStatistics(List<string> libraryIds);
        Task<ShowStatistics> CalculateShowStatistics(List<string> libraryIds);
        Task<ListContainer<ShowCollectionRow>> GetCollectedRows(IReadOnlyList<string> libraryIds, int page);
        Task<List<ShowCollectionRow>> CalculateCollectedRows(IReadOnlyList<string> libraryIds);
        bool TypeIsPresent();
        Task<Page<SqlShow>> GetShowPage(int skip, int take, string sortField, string sortOrder, Filter[] filters, bool requireTotalCount, List<string> libraryIds);
        Task<SqlShow> GetShow(string id);
    }
}
