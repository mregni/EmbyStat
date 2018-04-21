namespace EmbyStat.Controllers.ViewModels.Task
{
    public class TaskTriggerInfoViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public long? TimeOfDayTicks { get; set; }
        public long? IntervalTicks { get; set; }
        public int? DayOfWeek { get; set; }
        public long? MaxRuntimeTicks { get; set; }
    }
}
