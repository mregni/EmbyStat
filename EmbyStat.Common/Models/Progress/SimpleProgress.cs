using System;

namespace EmbyStat.Common.Models.Progress
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
