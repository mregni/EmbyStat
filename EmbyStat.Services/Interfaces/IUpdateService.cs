using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Interfaces
{
    public interface IUpdateService
    {
        void CheckForUpdate();
        void UpdateServer();
    }
}
