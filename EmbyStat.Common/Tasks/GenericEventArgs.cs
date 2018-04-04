using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Tasks
{
    public class GenericEventArgs<T> : EventArgs
    {
        public T Argument { get; set; }

        public GenericEventArgs(T arg)
        {
            Argument = arg;
        }

        public GenericEventArgs()
        {
        }
    }
}
