namespace EmbyStat.Controllers.HelperClasses
{
    public class TopCardViewModel
    {
        public string Title { get; set; }
        public string Unit { get; set; }
        public bool UnitNeedsTranslation { get; set; }
        public int ValueType { get; set; }
        public TopCardItemViewModel[] Values { get; set; }
    }

    public class TopCardItemViewModel
    {
        public string MediaId { get; set; }
        public string Image { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
    }
}
