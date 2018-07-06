using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Tasks.Interface
{
    public interface IProgressLogger
    {
        void LogInformation(string value);
        void LogWarning(string value);
        void LogError(Exception e, string value);
    }
}
