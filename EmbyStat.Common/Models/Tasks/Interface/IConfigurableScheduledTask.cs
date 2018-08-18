namespace EmbyStat.Common.Models.Tasks.Interface
{
    public interface IConfigurableScheduledTask
    {
        bool IsEnabled { get; }
        bool IsLogged { get; }
    }
}
