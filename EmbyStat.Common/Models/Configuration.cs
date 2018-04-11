using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models
{
    public class Configuration
    {
	    [Key]
	    public string Id { get; set; }
	    [Required]
	    public bool WizardFinished { get; set; }
	    public string AccessToken { get; set; }
	    public string EmbyUserName { get; set; }
	    public string EmbyServerAddress { get; set; }
	    public string Username { get; set; }
	    public string Language { get; set; }
	    public string ServerName { get; set; }
        public string EmbyUserId { get; set; }
	}
}
