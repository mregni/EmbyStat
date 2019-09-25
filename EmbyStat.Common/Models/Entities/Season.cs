using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities
{
    public class Season : Media
    {
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
        public LocationType LocationType { get; set; }
    }
}
