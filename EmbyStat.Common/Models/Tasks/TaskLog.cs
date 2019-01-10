namespace EmbyStat.Common.Models.Tasks
{
    public class TaskLog
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
