using System;

namespace EmbyStat.Controllers.Tasks
{
    public class TaskResultViewModel
    {
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Id { get; set; }
        public string ErrorMessage { get; set; }
        public string LongErrorMessage { get; set; }
    }
}
