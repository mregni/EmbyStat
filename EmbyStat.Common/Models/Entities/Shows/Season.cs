using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities.Shows
{
    public class Season : Media
    {
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
        public LocationType LocationType { get; set; }
        public ICollection<Episode> Episodes { get; set; }
        public Show Show { get; set; }
        public string ShowId { get; set; }
    }
}
