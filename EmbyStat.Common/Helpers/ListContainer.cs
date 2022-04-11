using System.Collections.Generic;

namespace EmbyStat.Common.Helpers;

public class ListContainer<T>
{
    public int TotalCount { get; set; }
    public IEnumerable<T> Data { get; set; }
}