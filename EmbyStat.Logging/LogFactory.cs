using System;

namespace EmbyStat.Logging
{
    public static class LogFactory
    {
        public static Logger CreateLoggerForType(Type type, string prefix)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger(type);
            return new Logger(logger, prefix);
        }
    }
}
