using EmbyStat.Common.Enums;

namespace EmbyStat.Common.Models.Cards;

public class Card
{
    public string Title { get; set; }
    public string Value { get; set; }
    public CardType Type { get; set; }
    public string Icon { get; set; }
}