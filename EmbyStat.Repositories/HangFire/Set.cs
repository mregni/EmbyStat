using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EmbyStat.Repositories.HangFire
{
	[Table("HangFire.Set")]
	public class Set
    {
		[Key]
		public int Id { get; set; }
	    public string Key { get; set; }
	    public float Score { get; set; }
	    public string Value { get; set; }
	    public DateTime? ExpireAt { get; set; }
	}
}
