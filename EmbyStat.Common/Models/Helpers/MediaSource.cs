using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Joins;

namespace EmbyStat.Common.Models.Helpers
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
