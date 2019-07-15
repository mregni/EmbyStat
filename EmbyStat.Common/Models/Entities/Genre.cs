using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities
{
    public class Genre
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
