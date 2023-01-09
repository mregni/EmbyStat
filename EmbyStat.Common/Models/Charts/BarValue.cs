namespace EmbyStat.Common.Models.Charts;

public class BarValue<T1, T2>
{
    public string Serie { get; set; }
    public T1 X { get; set; }
    public T2 Y { get; set; }
}