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
        ShowStatistics GetStatistics(List<string> libraryIds);
        ShowStatistics CalculateShowStatistics(List<string> libraryIds);
        ShowStatistics CalculateShowStatistics(string libraryId);
        ListContainer<ShowCollectionRow> GetCollectedRows(List<string> libraryIds, int page);
        List<ShowCollectionRow> CalculateCollectedRows(List<string> libraryIds);
        List<ShowCollectionRow> CalculateCollectedRows(string libraryId);
        bool TypeIsPresent();
        Task<Page<ShowRow>> GetShowPage(int skip, int take, string sort, Filter[] filters, bool requireTotalCount, List<string> libraryIds);
        SqlShow GetShow(string id);
    }
}
