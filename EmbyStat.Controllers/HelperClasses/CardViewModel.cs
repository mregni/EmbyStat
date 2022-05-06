namespace EmbyStat.Controllers.HelperClasses;

public class CardViewModel<T>
{
    public string Title { get; set; }
    public T Value { get; set; }
    public string Type { get; set; }
    public string Icon { get; set; }
}