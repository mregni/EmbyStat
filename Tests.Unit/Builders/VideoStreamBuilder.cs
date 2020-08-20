
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
                Width = 1000
            };
        }

        public VideoStreamBuilder AddWidth(int? width)
        {
            _stream.Width = width;
            return this;
        }

        public VideoStreamBuilder AddHeight(int? height)
        {
            _stream.Height = height;
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
