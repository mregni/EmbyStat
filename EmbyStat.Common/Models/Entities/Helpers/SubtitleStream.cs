using System;
using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class SubtitleStream
    {
        [Key]
        public string Id { get; set; }
        public string Codec { get; set; }
        public string DisplayTitle { get; set; }
        public bool IsDefault { get; set; }
        public string Language { get; set; }
        public Video Video { get; set; }
        public string VideoId { get; set; }
    }
}
