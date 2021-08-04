using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;

namespace EmbyStat.Common.Models.Net
{
    public class BaseItemDto
    {
        public string Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public Dictionary<ImageType, string> ImageTags { get; set; }
        public string[] BackdropImageTags { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string Path { get; set; }
        public DateTime? PremiereDate { get; set; }
        public int? ProductionYear { get; set; }
        public string SortName { get; set; }
        public string Status { get; set; }
        public string OriginalTitle { get; set; }
        public string Container { get; set; }
        public string MediaType { get; set; }
        public BaseMediaSourceInfo[] MediaSources { get; set; }
        public BaseMediaStream[] MediaStreams { get; set; }
        public Video3DFormat? Video3DFormat { get; set; }
        public float? CommunityRating { get; set; }
        public Dictionary<string, string> ProviderIds { get; set; }
        public long? RunTimeTicks { get; set; }
        public string OfficialRating { get; set; }
        public BaseItemPerson[] People { get; set; }
        public string[] Genres { get; set; }
        public int? IndexNumber { get; set; }
        public int? IndexNumberEnd { get; set; }
        public string Type { get; set; }
        public string SeriesName { get; set; }
        public string CollectionType { get; set; }
        public string Etag { get; set; }
        public string Overview { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
