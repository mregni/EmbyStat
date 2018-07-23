using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Tasks
{
    public class ProgressLog
    {
        public string Value { get; set; }
        public ProgressLogType Type { get; set; }
    }

    public enum ProgressLogType
    {
        Normal = 0,
        Warning = 1,
        Error = 2
    }
}
