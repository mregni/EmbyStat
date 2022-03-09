using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EmbyStat.Services.Models.Logs;

namespace EmbyStat.Services.Interfaces
{
    public interface ILogService
    {
        List<LogFile> GetLogFileList();
        Task<MemoryStream> GetLogStream(string fileName, bool anonymous);
    }
}
