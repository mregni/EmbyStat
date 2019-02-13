namespace EmbyStat.Services.Models.Stat
{
    public class Card<T>
    {
        public string Title { get; set; }
        public T Value { get; set; }
    }
}
