namespace EmbyStat.Controllers.ViewModels.Configuration
{
    public class ConfigurationViewModel
    {
	    public bool WizardFinished { get; set; }
	    public string AccessToken { get; set; }
	    public string EmbyUserName { get; set; }
	    public string EmbyServerAddress { get; set; }
	    public string Username { get; set; }
	    public string Language { get; set; }
	    public string ServerName { get; set; }
        public string EmbyUserId { get; set; }
        public int ToShortMovie { get; set; }
	}
}
