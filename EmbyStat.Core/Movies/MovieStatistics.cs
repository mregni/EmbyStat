using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Core.Movies;

public class MovieStatistics
{
    public List<Card> Cards { get; set; }
    public List<TopCard> TopCards { get; set; }
    public List<Chart> Charts { get; set; }
    public List<MultiChart> ComplexCharts { get; set; }
    public IEnumerable<ShortMovie> Shorts { get; set; }
    public IEnumerable<SuspiciousMovie> NoImdb { get; set; }
    public IEnumerable<SuspiciousMovie> NoPrimary { get; set; }
}