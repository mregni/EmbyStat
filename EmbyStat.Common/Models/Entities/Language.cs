using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities
{
    public class Language
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
