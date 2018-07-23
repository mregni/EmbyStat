using System;
using Serilog;

namespace EmbyStat.Common.Progress
{
    public class SimpleProgress<T> : IProgress<T>
    {
        public event EventHandler<T> ProgressChanged;

        public void Report(T value)
        {
            ProgressChanged?.Invoke(this, value);
        }
    }
}
