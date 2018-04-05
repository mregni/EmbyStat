using System;

namespace EmbyStat.Common.Progress
{
    public class SimpleProgress<T> : IProgress<T>
    {
        public event EventHandler<T> ProgressChanged;

        public void Report(T value)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, value);
            }
        }
    }
}
