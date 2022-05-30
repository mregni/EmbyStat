namespace EmbyStat.Core.Logging.Interfaces;

public interface ILogService
{
    List<LogFile> GetLogFileList();
    Task<MemoryStream> GetLogStream(string fileName, bool anonymous);
}