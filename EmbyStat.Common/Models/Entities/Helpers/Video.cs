using System.Collections.Generic;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class Video : Extra
    {
        public string Container { get; set; }
        public string MediaType { get; set; }
        public List<MediaSource> MediaSources { get; set; }
        public List<VideoStream> VideoStreams { get; set; }
        public List<AudioStream> AudioStreams { get; set; }
        public List<SubtitleStream> SubtitleStreams { get; set; }

        public Video()
        {
            MediaSources = new List<MediaSource>();
            VideoStreams = new List<VideoStream>();
            AudioStreams = new List<AudioStream>();
            SubtitleStreams = new List<SubtitleStream>();
        }
    }
}
