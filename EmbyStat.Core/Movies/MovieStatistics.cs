using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;

namespace EmbyStat.Core.Movies;

public class MovieStatistics
{
    public IEnumerable<Card> Cards { get; set; }
    public IEnumerable<TopCard> TopCards { get; set; }
    public IEnumerable<Chart> Charts { get; set; }
    public IEnumerable<MultiChart> ComplexCharts { get; set; }
    public IEnumerable<ShortMovie> Shorts { get; set; }
    public IEnumerable<SuspiciousMovie> NoImdb { get; set; }
    public IEnumerable<SuspiciousMovie> NoPrimary { get; set; }
}