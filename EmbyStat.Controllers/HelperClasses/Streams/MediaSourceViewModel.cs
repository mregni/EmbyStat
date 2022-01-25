using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbyStat.Controllers.HelperClasses
{
    public class MediaSourceViewModel
    {
        public int Id { get; set; }
        public int? BitRate { get; set; }
        public string Container { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }
        public long? RunTimeTicks { get; set; }
        public double SizeInMb { get; set; }
    }
}
