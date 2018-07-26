using System;

namespace EmbyStat.Services.Models.Emby
{
    public class EmbyToken
    {
	    public string Token { get; set; }
		public string Username { get; set; }
		public bool IsAdmin { get; set; }
        public Guid Id { get; set; }
    }
}
