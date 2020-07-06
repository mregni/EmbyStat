using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.Filters
{
    public class FilterValuesViewModel
    {
        public string Id { get; set; }
        public string Field { get; set; }
        public LabelValuePairViewModel[] Values { get; set; }
    }
}
