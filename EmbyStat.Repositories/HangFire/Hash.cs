using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EmbyStat.Repositories.HangFire
{
	[Table("HangFire.Hash")]
	public class Hash
    {
		[Key] 
		public int Id { get; set; }
	    public string Key { get; set; }
	    public string Field { get; set; }
	    public string Value { get; set; }
	    public DateTime? ExpireAt { get; set; }
    }
}
