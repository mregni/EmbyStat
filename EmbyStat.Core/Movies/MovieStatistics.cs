using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;

namespace EmbyStat.Core.Movies;

public class MovieStatistics
{
    public List<Card<string>> Cards { get; set; }
    public List<TopCard> TopCards { get; set; }
    public List<Chart> Charts { get; set; }
    public IEnumerable<ShortMovie> Shorts { get; set; }
    public IEnumerable<SuspiciousMovie> NoImdb { get; set; }
    public IEnumerable<SuspiciousMovie> NoPrimary { get; set; }
}