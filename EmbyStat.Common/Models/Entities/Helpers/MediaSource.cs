using System;
using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class MediaSource
    {
        [Key]
        public string Id { get; set; }
        public long? BitRate { get; set; }
        public string Container { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }
        public long? RunTimeTicks { get; set; }
        public string VideoType { get; set; }
        public Video Video { get; set; }
        public Guid VideoId { get; set; }
    }
}
