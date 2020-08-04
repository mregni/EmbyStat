using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces
{
    public interface IFilterService
    {
        FilterValues GetFilterValues(LibraryType type, string field, string[] libraryIds);
        FilterValues CalculateFilterValues(LibraryType type, string field, string[] libraryIds);
    }
}
