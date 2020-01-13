using System.Collections.Generic;
using System.IO;
using EmbyStat.Services.Models.Logs;

namespace EmbyStat.Services.Interfaces
{
    public interface ILogService
    {
        List<LogFile> GetLogFileList();
        Stream GetLogStream(string fileName, bool anonymous);
    }
}
