using System;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models.Joins
{
    public class ExtraPerson
    {
        [Key]
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid ExtraId { get; set; }
        public Extra Extra { get; set; }
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
    }
}
