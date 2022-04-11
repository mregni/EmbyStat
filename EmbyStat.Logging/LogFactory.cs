using System;
using NLog;

namespace EmbyStat.Logging;

public static class LogFactory
{
    public static Logger CreateLoggerForType(Type type, string prefix)
    {
        var logger = LogManager.GetCurrentClassLogger(type);
        return new Logger(logger, prefix);
    }
}