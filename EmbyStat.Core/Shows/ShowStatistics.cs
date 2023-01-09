using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;

namespace EmbyStat.Core.Shows;

public class ShowStatistics
{
    public IEnumerable<Card> Cards { get; set; }
    public IEnumerable<TopCard> TopCards { get; set; }
    public IEnumerable<Chart> BarCharts { get; set; }
    public IEnumerable<MultiChart> ComplexCharts { get; set; }
    public IEnumerable<Chart> PieCharts { get; set; }
}