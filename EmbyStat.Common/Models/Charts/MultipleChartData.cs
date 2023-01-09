using System.Collections.Generic;

namespace EmbyStat.Common.Models.Charts;

public class MultipleChartData<T>
{
    public string Label { get; set; }
    public Dictionary<string, T> Value { get; set; }
}