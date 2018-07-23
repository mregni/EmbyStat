using System.Collections.Generic;

namespace EmbyStat.Controllers.ViewModels.Configuration
{
    public class ConfigurationViewModel
    {
        public string Id { get; set; }
	    public bool WizardFinished { get; set; }
	    public string AccessToken { get; set; }
	    public string EmbyUserName { get; set; }
	    public string EmbyServerAddress { get; set; }
	    public string Username { get; set; }
	    public string Language { get; set; }
	    public string ServerName { get; set; }
        public string EmbyUserId { get; set; }
        public int ToShortMovie { get; set; }
        public List<int> MovieCollectionTypes { get; set; } 
        public List<int> ShowCollectionTypes { get; set; }
    }
}
