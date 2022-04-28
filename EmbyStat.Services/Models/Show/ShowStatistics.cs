using System.Collections.Generic;
using EmbyStat.Services.Models.Cards;
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Show;

public class ShowStatistics
{
    public List<Card<string>> Cards { get; set; }
    public List<TopCard> TopCards { get; set; }
    public List<Chart> BarCharts { get; set; }
    public List<Chart> PieCharts { get; set; }
}