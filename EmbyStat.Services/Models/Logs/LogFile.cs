using System;

namespace EmbyStat.Services.Models.Logs
{
    public class LogFile
    {
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long Size { get; set; }
    }
}
