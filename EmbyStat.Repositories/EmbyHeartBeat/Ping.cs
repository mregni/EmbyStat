using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmbyStat.Repositories.EmbyHeartBeat
{
	public class Ping
	{
		[Key] 
		public string Id { get; set; }

		public DateTime Time { get; set; }
		public bool Success { get; set; }
	}
}
