using System;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Entities.Helpers;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Common.Models.Entities.Joins
{
    public class ExtraPerson
    {
        [Key]
        public Guid Id { get; set; }
        public PersonType Type { get; set; }
        public string ExtraId { get; set; }
        public Extra Extra { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
    }
}
