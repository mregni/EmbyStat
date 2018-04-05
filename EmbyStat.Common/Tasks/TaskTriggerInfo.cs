using System;
using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Tasks
{
    public class TaskTriggerInfo
    {
        [Key]
        public string Id { get; set; }
        public string Type { get; set; }
        public long? TimeOfDayTicks { get; set; }
        public long? IntervalTicks { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public long? MaxRuntimeTicks { get; set; }
    }
}
