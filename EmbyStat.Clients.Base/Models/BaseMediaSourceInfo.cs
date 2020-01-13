
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Base.Models
{
    public class BaseMediaSourceInfo
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public int? Bitrate { get; set; }
        public string Container { get; set; }
        public MediaProtocol Protocol { get; set; }
        public long? RunTimeTicks { get; set; }
        public long? Size { get; set; }
    }
}
