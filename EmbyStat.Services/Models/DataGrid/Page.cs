using System.Collections.Generic;

namespace EmbyStat.Services.Models.DataGrid;

public class Page<T>
{
    public IEnumerable<T> Data { get; set; }
    public int TotalCount { get; set; }

    public Page(IEnumerable<T> data)
    {
        Data = data;
    }
}