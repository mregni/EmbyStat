using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite.Shows;
using EmbyStat.Services.Models.DataGrid;
using EmbyStat.Services.Models.Show;

namespace EmbyStat.Services.Interfaces
{
    public interface IShowService
    {
        Task<List<Library>> GetShowLibraries();
        Task<ShowStatistics> GetStatistics();
        Task<ShowStatistics> CalculateShowStatistics();
        bool TypeIsPresent();
        Task<Page<SqlShow>> GetShowPage(int skip, int take, string sortField, string sortOrder, Filter[] filters, bool requireTotalCount);
        Task<SqlShow> GetShow(string id);
    }
}
