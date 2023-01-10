using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities.Statistics;
using Newtonsoft.Json;

namespace EmbyStat.Core.Movies;

public class MovieStatistics
{
    public IEnumerable<Card> Cards { get; set; }
    public IEnumerable<TopCard> TopCards { get; set; }
    public IEnumerable<Chart> Charts { get; set; }
    public IEnumerable<ComplexChart> ComplexCharts { get; set; }
    public IEnumerable<ShortMovie> Shorts { get; set; }
    public IEnumerable<SuspiciousMovie> NoImdb { get; set; }
    public IEnumerable<SuspiciousMovie> NoPrimary { get; set; }

    public MovieStatistics(StatisticPage page)
    {
        Cards = page.PageCards
            .Where(x => x.StatisticCard.CardType == StatisticCardType.Card &&
                        !string.IsNullOrWhiteSpace(x.StatisticCard.Data))
            .OrderBy(x => x.Order)
            .Select(x => JsonConvert.DeserializeObject<Card>(x.StatisticCard.Data))
            .Where(x => x != null)!;
        TopCards = page.PageCards
            .Where(x => x.StatisticCard.CardType == StatisticCardType.TopCard &&
                        !string.IsNullOrWhiteSpace(x.StatisticCard.Data))
            .OrderBy(x => x.Order)
            .Select(x => JsonConvert.DeserializeObject<TopCard>(x.StatisticCard.Data))
            .Where(x => x != null)!;
        Charts = page.PageCards
            .Where(x => x.StatisticCard.CardType == StatisticCardType.BarChart &&
                        !string.IsNullOrWhiteSpace(x.StatisticCard.Data))
            .OrderBy(x => x.Order)
            .Select(x => JsonConvert.DeserializeObject<Chart>(x.StatisticCard.Data))
            .Where(x => x != null)!;
        ComplexCharts = page.PageCards
            .Where(x => x.StatisticCard.CardType == StatisticCardType.ComplexChart &&
                        !string.IsNullOrWhiteSpace(x.StatisticCard.Data))
            .OrderBy(x => x.Order)
            .Select(x => JsonConvert.DeserializeObject<ComplexChart>(x.StatisticCard.Data))
            .Where(x => x != null)!;
    }
}