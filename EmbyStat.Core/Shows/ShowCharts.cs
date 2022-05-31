using EmbyStat.Common.Models.Charts;

namespace EmbyStat.Core.Shows;

public class ShowCharts
{
    public List<Chart> BarCharts { get; set; }
    public List<Chart> PieCharts { get; set; }

    public ShowCharts()
    {
        BarCharts = new List<Chart>();
        PieCharts = new List<Chart>();
    }
}