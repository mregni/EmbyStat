using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;

namespace EmbyStat.Services.Models.Cards
{
    public class TopCard
    {
        public string MediaId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Unit { get; set; }
        public bool UnitNeedsTranslation { get; set; }
        public ValueType ValueType { get; set; }
        public LabelValuePair[] Values { get; set; }
    }
}
