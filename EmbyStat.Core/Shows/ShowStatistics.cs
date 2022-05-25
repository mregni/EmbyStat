using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;

namespace EmbyStat.Core.Shows;

public class ShowStatistics
{
    public List<Card<string>> Cards { get; set; }
    public List<TopCard> TopCards { get; set; }
    public List<Chart> BarCharts { get; set; }
    public List<Chart> PieCharts { get; set; }
}