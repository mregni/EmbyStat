using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Streams;

namespace EmbyStat.Common.SqLite.Movies
{
    public class SqlMovie : SqlExtra
    {
        public string OriginalTitle { get; set; }
        public string Container { get; set; }
        public string MediaType { get; set; }
        public ICollection<SqlMediaSource> MediaSources { get; set; }
        public ICollection<SqlVideoStream> VideoStreams { get; set; }
        public ICollection<SqlAudioStream> AudioStreams { get; set; }
        public ICollection<SqlSubtitleStream> SubtitleStreams { get; set; }
        public ICollection<SqlMoviePerson> MoviePeople { get; set; }
        public Video3DFormat Video3DFormat { get; set; }

        public override bool Equals(object? other)
        {
            if (other is SqlMovie media)
            {
                return Id == media.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
