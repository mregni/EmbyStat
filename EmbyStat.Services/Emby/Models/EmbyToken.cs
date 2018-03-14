using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Emby.Models
{
    public class EmbyToken
    {
	    public string Token { get; set; }
		public string Username { get; set; }
		public bool IsAdmin { get; set; }
    }
}
