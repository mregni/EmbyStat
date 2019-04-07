using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.HelperClasses.Graphs
{
    public class DataSetViewModel<T>
    {
        public string Label { get; set; }
        public T[] Data { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public string HoverBackgroundColor { get; set; }
        public string HoverBorderColor { get; set; }
        public int BorderWidth { get; set; }
    }
}
