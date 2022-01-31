using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.SqLite.Helpers;
using EmbyStat.Common.SqLite.Streams;

namespace EmbyStat.Common.SqLite.Shows
{
    public class SqlEpisode : SqlMedia
    {
        public float? CommunityRating { get; set; }
        public string IMDB { get; set; }
        public int? TMDB { get; set; }
        public string TVDB { get; set; }
        public long? RunTimeTicks { get; set; }
        public string OfficialRating { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Container { get; set; }
        public string MediaType { get; set; }
        public ICollection<SqlMediaSource> MediaSources { get; set; }
        public ICollection<SqlVideoStream> VideoStreams { get; set; }
        public ICollection<SqlAudioStream> AudioStreams { get; set; }
        public ICollection<SqlSubtitleStream> SubtitleStreams { get; set; }
        public ICollection<SqlShowSqlPerson> MoviePeople { get; set; }
        public ICollection<SqlGenre> Genres { get; set; }
        public Video3DFormat Video3DFormat { get; set; }
        public float? DvdEpisodeNumber { get; set; }
        public int? DvdSeasonNumber { get; set; }
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
        public string ShowName { get; set; }
        public string ShowId { get; set; }
        public int? SeasonIndexNumber { get; set; }
        public LocationType LocationType { get; set; }
        public SqlSeason Season { get; set; }
        public string SeasonId { get; set; }

    }
}
