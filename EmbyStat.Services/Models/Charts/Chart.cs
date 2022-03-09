namespace EmbyStat.Services.Models.Charts
{
    public class Chart
    {
        public string Title { get; set; }
        public SimpleChartData[] DataSets { get; set; }
        public int SeriesCount { get; set; }
    }
}
