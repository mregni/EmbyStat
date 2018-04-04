using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Tasks
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
