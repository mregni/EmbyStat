using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities
{
    public class Genre
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<Media> Media { get; set; }
    }
}
