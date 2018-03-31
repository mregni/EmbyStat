using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Repositories.EmbyDrive
{
    public class Drives
    {
	    [Key]
		public string Id { get; set; }
		public string Name { get; set; }
	    public string Path { get; set; }
	    public string Type { get; set; }
	}
}
