using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Streams;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public abstract class Video : Extra
    {
        public string Container { get; set; }
        public Video3DFormat Video3DFormat { get; set; }
        public ICollection<MediaSource> MediaSources { get; set; }
        public ICollection<VideoStream> VideoStreams { get; set; }
        public ICollection<AudioStream> AudioStreams { get; set; }
        public ICollection<SubtitleStream> SubtitleStreams { get; set; }
    }
}
