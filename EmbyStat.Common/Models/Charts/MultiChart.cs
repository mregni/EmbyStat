namespace EmbyStat.Common.Models.Charts;

public class MultiChart
{
    public string Title { get; set; }
    public string DataSets { get; set; }
    public string[] Series { get; set; }
    public string FormatString { get; set; }
}