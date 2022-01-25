using System;
using System.Collections.Generic;

namespace EmbyStat.Common.Models
{
    public class LabelValuePair : IEqualityComparer<LabelValuePair>
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public bool Equals(LabelValuePair x, LabelValuePair y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Label == y.Label;
        }

        public int GetHashCode(LabelValuePair obj)
        {
            return (obj.Label != null ? obj.Label.GetHashCode() : 0);
        }
    }
}
