using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Tasks.Enum;

namespace EmbyStat.Common.Tasks
{
    public class TaskStatus
    {
        public string Id { get; set; }
        public TaskState State { get; set; }
        public double? CurrentProgress { get; set; }
    }
}
