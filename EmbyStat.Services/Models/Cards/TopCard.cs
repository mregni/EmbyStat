using EmbyStat.Common.Models;

namespace EmbyStat.Services.Models.Cards
{
    public class TopCard
    {
        public string Title { get; set; }
        public string Unit { get; set; }
        public bool UnitNeedsTranslation { get; set; }
        public ValueTypeEnum ValueType { get; set; }
        public TopCardItem[] Values { get; set; }
    }

    public class TopCardItem
    {
        public string MediaId { get; set; }
        public string Image { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public enum ValueTypeEnum
    {
        None = 0,
        Ticks = 1,
        Date = 2
    }
}
