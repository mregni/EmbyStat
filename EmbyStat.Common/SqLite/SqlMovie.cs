using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.SqLite
{
    public class SqlMovie
    {
        public string Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Primary { get; set; }
        public string Thumb { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string Path { get; set; }
        public DateTime? PremiereDate { get; set; }
        public int? ProductionYear { get; set; }
        public string SortName { get; set; }
        public string CollectionId { get; set; }
        public string OriginalTitle { get; set; }
        public string Container { get; set; }
        public string MediaType { get; set; }
        public float? CommunityRating { get; set; }
        public string IMDB { get; set; }
        public int? TMDB { get; set; }
        public string TVDB { get; set; }
        public long? RunTimeTicks { get; set; }
        public string OfficialRating { get; set; }
        public ICollection<SqlMediaSource> MediaSources { get; set; }
        public ICollection<SqlVideoStream> VideoStreams { get; set; }
        public ICollection<SqlAudioStream> AudioStreams { get; set; }
        public ICollection<SqlSubtitleStream> SubtitleStreams { get; set; }
        public ICollection<SqlMoviePerson> MoviePeople { get; set; }
        public ICollection<SqlGenre> Genres { get; set; }
        public Video3DFormat Video3DFormat { get; set; }

        public override bool Equals(object? other)
        {
            if (other is Media media)
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
