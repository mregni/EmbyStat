using LiteDB;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class MediaSource
    {
        [BsonId]
        public string Id { get; set; }
        public long? BitRate { get; set; }
        public string Container { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }
        public long? RunTimeTicks { get; set; }
    }
}
