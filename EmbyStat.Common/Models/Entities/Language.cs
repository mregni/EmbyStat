using LiteDB;

namespace EmbyStat.Common.Models.Entities
{
    public class Language
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
