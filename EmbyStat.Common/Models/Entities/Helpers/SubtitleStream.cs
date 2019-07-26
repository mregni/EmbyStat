using LiteDB;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class SubtitleStream
    {
        [BsonId]
        public string Id { get; set; }
        public string Codec { get; set; }
        public string DisplayTitle { get; set; }
        public bool IsDefault { get; set; }
        public string Language { get; set; }
    }
}
