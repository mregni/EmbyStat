using System;

namespace EmbyStat.Controllers.ViewModels.Logs
{
    public class LogFileViewModel
    {
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long Size { get; set; }
    }
}
