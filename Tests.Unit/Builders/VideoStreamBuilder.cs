
using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models.Entities.Helpers;

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

        public VideoStreamBuilder AddWidth(int? width)
        {
            _stream.Width = width;
            return this;
        }
        public VideoStreamBuilder AddCodec(string codec)
        {
            _stream.Codec = codec;
            return this;
        }
        public VideoStreamBuilder AddVideoRange(string videoRange)
        {
            _stream.VideoRange = videoRange;
            return this;
        }

        public VideoStreamBuilder AddHeight(int? height)
        {
            _stream.Height = height;
            return this;
        }

        public VideoStreamBuilder AddBitDepth(int? depth)
        {
            _stream.BitDepth = depth;
            return this;
        }

        public VideoStreamBuilder AddAverageFrameRate(float? frameRate)
        {
            _stream.AverageFrameRate = frameRate;
            return this;
        }

        public VideoStream Build()
        {
            return _stream;
        }
    }
}
