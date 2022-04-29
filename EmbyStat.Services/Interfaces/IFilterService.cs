using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces;

public interface IFilterService
{
    Task<FilterValues> GetFilterValues(LibraryType type, string field);
    FilterValues CalculateFilterValues(LibraryType type, string field);
}