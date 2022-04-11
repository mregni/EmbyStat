using System;

namespace EmbyStat.Logging;

public class Logger
{
    private string Prefix { get; }
    private NLog.Logger NLogLogger { get; }

    public Logger(NLog.Logger logger, string prefix)
    {
        Prefix = prefix;
        NLogLogger = logger;
    }

    public void Debug(string message)
    {
        NLogLogger.Debug($"{Prefix}\t{message}");
    }

    public void Info(string message)
    {
        NLogLogger.Info($"{Prefix}\t{message}");
    }

    public void Error(string message)
    {
        NLogLogger.Warn($"{Prefix}\t{message}");
    }

    public void Error(Exception e, string message)
    {
        NLogLogger.Error(e, message);
    }

    public void Error(Exception e)
    {
        NLogLogger.Error(e);
    }

    public void Warn(string message)
    {
        NLogLogger.Warn($"{Prefix}\t{message}");
    }

    public void Warn(Exception e, string message)
    {
        NLogLogger.Error(e, message);
    }
}