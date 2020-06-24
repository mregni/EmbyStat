using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Services.Models;

namespace EmbyStat.Controllers.HelperClasses
{
    public class TopCardViewModel
    {
        public string MediaId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Unit { get; set; }
        public bool UnitNeedsTranslation { get; set; }
        public int ValueType { get; set; }
        public LabelValuePairViewModel[] Values { get; set; }
    }
}
