using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Api.EmbyClient.Model
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
















