using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Tasks.Interface;
using Serilog;

namespace EmbyStat.Common.Tasks
{
    public class ProgressLogger : IProgressLogger
    {
        public event EventHandler<ProgressLog> ProgressLogged;

        public void LogInformation(string prefix, string value)
        {
            Log.Information($"{prefix}\t{value}");
            ProgressLogged?.Invoke(this, new ProgressLog { Type = ProgressLogType.Normal, Value = value });
        }

        public void LogWarning(string prefix, string value)
        {
            Log.Warning($"{prefix}\t{value}");
            ProgressLogged?.Invoke(this, new ProgressLog { Type = ProgressLogType.Warning, Value = value });
        }

        public void LogError(string prefix, Exception e, string value)
        {
            Log.Error(e, $"{prefix}\t{value}");
            ProgressLogged?.Invoke(this, new ProgressLog { Type = ProgressLogType.Error, Value = value });
        }
    }
}
