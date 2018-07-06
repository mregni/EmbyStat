using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Tasks.Interface;
using Serilog;

namespace EmbyStat.Common.Tasks
{
    public class ProgressLogger : IProgressLogger
    {
        public event EventHandler<string> ProgressLogged;

        public void LogInformation(string value)
        {
            Log.Information(value);
            ProgressLogged?.Invoke(this, value);
        }

        public void LogWarning(string value)
        {
            Log.Warning(value);
            ProgressLogged?.Invoke(this, value);
        }

        public void LogError(Exception e, string value)
        {
            Log.Error(e, value);
            ProgressLogged?.Invoke(this, value);
        }
    }
}
