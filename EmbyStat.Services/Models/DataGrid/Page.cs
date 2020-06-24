using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.DataGrid
{
    public class Page<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
