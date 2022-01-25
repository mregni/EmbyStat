using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbyStat.Controllers.HelperClasses.Streams
{
    public class SubtitleStreamViewModel
    {
        public int Id { get; set; }
        public string Codec { get; set; }
        public string DisplayTitle { get; set; }
        public bool IsDefault { get; set; }
        public string Language { get; set; }
    }
}
