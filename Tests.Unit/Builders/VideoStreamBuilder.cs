using EmbyStat.Common.Models.Entities.Streams;

namespace Tests.Unit.Builders
{
    public class VideoStreamBuilder
    {
        private readonly VideoStream _stream;

        public VideoStreamBuilder()
        {
            _stream = new VideoStream
            {
                AverageFrameRate = 25,
                Height = 1000,
                Width = 1000,
                Codec = "h264",
                BitDepth = 8,
                VideoRange = "SDR"
            };
        }

        public VideoStream Build()
        {
            return _stream;
        }
    }
}
