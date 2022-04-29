using System.Collections.Generic;
using EmbyStat.Services.Models.Cards;
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services.Models.Movie;

public class MovieStatistics
{
    public List<Card<string>> Cards { get; set; }
    public List<TopCard> TopCards { get; set; }
    public List<Chart> Charts { get; set; }
    public IEnumerable<ShortMovie> Shorts { get; set; }
    public IEnumerable<SuspiciousMovie> NoImdb { get; set; }
    public IEnumerable<SuspiciousMovie> NoPrimary { get; set; }
}