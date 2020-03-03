using EmbyStat.Common.Enums;
using LiteDB;

namespace EmbyStat.Common.Models.Entities
{
    public class Library
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string PrimaryImage { get; set; }
        public LibraryType Type { get; set; }
    }
}
