using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Logs
{
    public class LogFile
    {
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long Size { get; set; }
    }
}
