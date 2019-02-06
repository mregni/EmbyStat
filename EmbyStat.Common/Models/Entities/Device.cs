using System;
using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities
{
    public class Device
    {
        [Key]
		public string Id { get; set; }
        public string Name { get; set; }
		public string AppName { get; set; }
		public string AppVersion { get; set; }
        public string LastUserId { get; set; }
        public DateTime DateLastActivity { get; set; }
        public string IconUrl { get; set; }
	}
}