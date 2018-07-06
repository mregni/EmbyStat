using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Api.Tvdb.Models
{
    public class UpdateShowModel
    {
        public string ShowId { get; set; }
        public IEnumerable<Episode> Episodes { get; set; }
    }
}
