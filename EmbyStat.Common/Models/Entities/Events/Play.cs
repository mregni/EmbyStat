using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using LiteDB;

namespace EmbyStat.Common.Models.Entities.Events
{
    public class Play
    {
        [BsonId]
        public Guid Id { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public PlayType Type { get; set; }
        public string MediaId { get; set; }
        public ICollection<PlayState> PlayStates { get; set; }
        public string SubtitleCodec { get; set; }
        public string SubtitleLanguage { get; set; }
        public string SubtitleDisplayTitle { get; set; }
        public string AudioCodec { get; set; }
        public string AudioLanguage { get; set; }
        public string AudioChannelLayout { get; set; }
        public string VideoCodec { get; set; }
        public int? VideoHeight { get; set; }
        public int? VideoWidth { get; set; }
        public double? VideoAverageFrameRate { get; set; }
        public double? VideoRealFrameRate { get; set; }
        public string VideoAspectRatio { get; set; }
        public string ParentId { get; set; }

        public Play()
        {
            PlayStates = new List<PlayState>();
        }
    }
}