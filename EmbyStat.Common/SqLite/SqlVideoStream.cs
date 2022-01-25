﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbyStat.Common.SqLite
{
    public class SqlVideoStream
    {
        public int Id { get; set; }
        public string AspectRatio { get; set; }
        public float? AverageFrameRate { get; set; }
        public int? BitRate { get; set; }
        public int? Channels { get; set; }
        public int? Height { get; set; }
        public string Language { get; set; }
        public int? Width { get; set; }
        public int? BitDepth { get; set; }
        public string Codec { get; set; }
        public bool IsDefault { get; set; }
        public string VideoRange { get; set; }
        public SqlMovie Movie { get; set; }
        public string MovieId { get; set; }
    }
}
