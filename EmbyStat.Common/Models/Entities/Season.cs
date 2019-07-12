using EmbyStat.Common.Models.Entities.Helpers;
using LiteDB;

namespace EmbyStat.Common.Models.Entities
{
    public class Season : Media
    {
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
    }
}
