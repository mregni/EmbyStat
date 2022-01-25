using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.SqLite
{
    public class SqlSubtitleStream
    {
        public int Id { get; set; }
        public string Codec { get; set; }
        public string DisplayTitle { get; set; }
        public bool IsDefault { get; set; }
        public string Language { get; set; }
        public SqlMovie Movie { get; set; }
        public string MovieId { get; set; }
    }
}
