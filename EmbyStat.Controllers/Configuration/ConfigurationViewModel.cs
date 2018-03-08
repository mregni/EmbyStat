using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Configuration
{
    public class ConfigurationViewModel
    {
	    public bool WizardFinished { get; set; }
	    public string AccessToken { get; set; }
	    public string EmbyUserName { get; set; }
	    public string EmbyServerAddress { get; set; }
	    public string Username { get; set; }
	    public string UserId { get; set; }
	    public string Language { get; set; }
	}
}
