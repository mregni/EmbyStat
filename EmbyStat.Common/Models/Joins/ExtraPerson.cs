using EmbyStat.Common.Models.Helpers;

namespace EmbyStat.Common.Models.Joins
{
    public class ExtraPerson
    {
        public string ExtraId { get; set; }
        public Extra Extra { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
    }
}
