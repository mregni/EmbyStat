using EmbyStat.Common.Enums;

namespace EmbyStat.Controllers.MediaServer
{
    public class LoginViewModel
    {
	    public string ApiKey { get; set; }
	    public string Address { get; set; }
        public ServerType Type { get; set; }
	}
}
