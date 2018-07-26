using System;
using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models.Joins
{
    public class ExtraPerson
    {
        public string Type { get; set; }
        public Guid ExtraId { get; set; }
        public Extra Extra { get; set; }
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
    }
}
