using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Tasks.Interface
{
    public interface IProgressLogger
    {
        void LogInformation(string prefix, string value);
        void LogWarning(string prefix, string value);
        void LogError(string prefix, Exception e, string value);
    }
}
