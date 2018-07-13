using System;
using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models
{
    public class Configuration
    {
	    [Key]
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
