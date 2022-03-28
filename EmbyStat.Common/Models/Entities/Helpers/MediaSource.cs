namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class MediaSource
    {
        public string Id { get; set; }
        public int? BitRate { get; set; }
        public string Container { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }
        public long? RunTimeTicks { get; set; }
        public double SizeInMb { get; set; }
    }
}
