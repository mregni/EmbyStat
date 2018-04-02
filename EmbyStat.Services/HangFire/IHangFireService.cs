using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.HangFire
{
    public interface IHangFireService
    {
	    List<BackgroundTask> GetTasks();
	    void FireTask(string id);
    }
}
