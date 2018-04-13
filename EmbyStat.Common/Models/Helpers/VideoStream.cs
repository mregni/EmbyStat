using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Helpers
{
    public class VideoStream
    {
        [Key]
        public string Id { get; set; }
        public string AspectRatio { get; set; }
        public float? AverageFrameRate { get; set; }
        public long? BitRate { get; set; }
        public int? Channels { get; set; }
        public int? Height { get; set; }
        public string Language { get; set; }
        public int? Width { get; set; }
        public Video Video { get; set; }
        public string VideoId { get; set; }
    }
}
