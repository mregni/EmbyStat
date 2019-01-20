using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Clients.EmbyClient.Model
{
    public class Drive
    {
        [Key]
        public string Id { get; set; }
	    public string Name { get; set; }
	    public string Path { get; set; }
	    public string Type { get; set; }
	}
}
















