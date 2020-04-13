using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace EmbyStat.Common.Helpers
{
    public static class LogLevelChanger
    {
        public static void SetNlogLogLevel(LogLevel level)
        {
            if (level == LogLevel.Off)
            {
                LogManager.DisableLogging();
            }
            else
            {
                if (!LogManager.IsLoggingEnabled())
                {
                    LogManager.EnableLogging();
                }

                foreach (var rule in LogManager.Configuration.LoggingRules)
                {
                    for (var i = level.Ordinal; i <= 5; i++)
                    {
                        rule.EnableLoggingForLevel(LogLevel.FromOrdinal(i));
                    }
                }
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
}