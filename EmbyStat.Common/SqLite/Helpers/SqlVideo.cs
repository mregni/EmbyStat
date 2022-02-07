using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Streams;

namespace EmbyStat.Common.SqLite.Helpers
{
    public abstract class SqlVideo : SqlExtra
    {
        public string Container { get; set; }
        public Video3DFormat Video3DFormat { get; set; }
        public ICollection<SqlMediaSource> MediaSources { get; set; }
        public ICollection<SqlVideoStream> VideoStreams { get; set; }
        public ICollection<SqlAudioStream> AudioStreams { get; set; }
        public ICollection<SqlSubtitleStream> SubtitleStreams { get; set; }
    }
}
