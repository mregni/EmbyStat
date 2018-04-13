using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Joins;

namespace EmbyStat.Common.Models
{
    public class Person
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public ICollection<ExtraPerson> ExtraPersons { get; set; }
    }
}
