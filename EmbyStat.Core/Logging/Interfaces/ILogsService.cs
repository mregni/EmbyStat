namespace EmbyStat.Core.Logging.Interfaces;

public interface ILogService
{
    IEnumerable<LogFile> GetLogFileList();
    Task<MemoryStream> GetLogStream(string fileName, bool anonymous);
}