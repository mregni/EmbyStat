using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Task
{
    public class BackgroundTaskViewModel
    {
	    public string Name { get; set; }
	    public int Cron { get; set; }
	    public int Type { get; set; }
	    public DateTime LastExecution { get; set; }
	    public DateTime NextExecution { get; set; }
	}
}
