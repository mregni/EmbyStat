using System;
using System.Collections.Generic;
using System.Text;

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
