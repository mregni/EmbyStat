using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.Entities.Statistics;
using Newtonsoft.Json;

namespace EmbyStat.Core.Shows;

public class ShowStatistics
{
    public IEnumerable<Card> Cards { get; set; }
    public IEnumerable<TopCard> TopCards { get; set; }
    public IEnumerable<Chart> BarCharts { get; set; }
    public IEnumerable<ComplexChart> ComplexCharts { get; set; }
    public IEnumerable<Chart> PieCharts { get; set; }

    public ShowStatistics(StatisticPage page)
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
        BarCharts = page.PageCards
            .Where(x => x.StatisticCard.CardType == StatisticCardType.BarChart &&
                        !string.IsNullOrWhiteSpace(x.StatisticCard.Data))
            .OrderBy(x => x.Order)
            .Select(x => JsonConvert.DeserializeObject<Chart>(x.StatisticCard.Data))
            .Where(x => x != null)!;
        PieCharts = page.PageCards
            .Where(x => x.StatisticCard.CardType == StatisticCardType.PieChart &&
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