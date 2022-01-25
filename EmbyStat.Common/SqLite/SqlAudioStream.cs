using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.SqLite
{
    public class SqlAudioStream
    {
        public int Id { get; set; }
        public int? BitRate { get; set; }
        public string ChannelLayout { get; set; }
        public int? Channels { get; set; }
        public string Codec { get; set; }
        public string Language { get; set; }
        public int? SampleRate { get; set; }
        public bool IsDefault { get; set; }
        public SqlMovie Movie { get; set; }
        public string MovieId { get; set; }
    }
}
