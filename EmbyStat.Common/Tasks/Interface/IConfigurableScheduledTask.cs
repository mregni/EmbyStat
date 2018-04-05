using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Tasks.Interface
{
    public interface IConfigurableScheduledTask
    {
        bool IsEnabled { get; }
        bool IsLogged { get; }
    }
}
