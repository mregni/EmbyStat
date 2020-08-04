using EmbyStat.Common.Enums;

namespace EmbyStat.Services.Models.Stat
{
    public class Card<T>
    {
        public string Title { get; set; }
        public T Value { get; set; }
        public CardType Type { get; set; }
        public string Icon { get; set; }
    }
}
