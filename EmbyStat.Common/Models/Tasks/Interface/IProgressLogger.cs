using System;

namespace EmbyStat.Common.Models.Tasks.Interface
{
    public interface IProgressLogger
    {
        void LogInformation(string prefix, string value);
        void LogWarning(string prefix, string value);
        void LogError(string prefix, Exception e, string value);
    }
}
