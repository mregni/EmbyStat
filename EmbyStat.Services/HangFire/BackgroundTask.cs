using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Repositories.HangFire;

namespace EmbyStat.Services.HangFire
{
    public class BackgroundTask
    {
		public string Name { get; set; }
		public int Cron { get; set; }
		public CronType Type { get; set; }
	    public DateTime LastExecution { get; set; }
	    public DateTime NextExecution { get; set; }

	    public BackgroundTask(string name, List<Hash> hashes)
	    {
		    Name = name;
		    NextExecution = Convert.ToDateTime(hashes.SingleOrDefault(x => x.Field == "NextExecution")?.Value);
		    LastExecution = Convert.ToDateTime(hashes.SingleOrDefault(x => x.Field == "LastExecution")?.Value);
			var cronStr = hashes.SingleOrDefault(x => x.Field == "Cron")?.Value;

		    Cron = GetCron(cronStr);
		    Type = GetCronType(cronStr);
	    }
	    private static int GetCron(string cronStr)
	    {
			return Convert.ToInt32(new String(cronStr.Where(Char.IsDigit).ToArray()));
		}

	    private static CronType GetCronType(string cronStr)
	    {
		    var arrayStr = cronStr.Split('*');
		    if (!string.IsNullOrEmpty(arrayStr[1])) return CronType.Minutely;
		    if (!string.IsNullOrEmpty(arrayStr[2])) return CronType.Hourly;
		    if (!string.IsNullOrEmpty(arrayStr[3])) return CronType.Dayly;
		    if (!string.IsNullOrEmpty(arrayStr[4])) return CronType.Weekly;
		    return !string.IsNullOrEmpty(arrayStr[5]) ? CronType.Monthly : CronType.Minutely;
	    }
	}

	public enum CronType
	{
		Minutely = 0,
		Hourly = 1,
		Dayly = 2,
		Weekly = 3,
		Monthly = 4
	}
}
