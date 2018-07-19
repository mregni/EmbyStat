using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Extentions
{
    public static class StringExtentions
    {
        public static bool ToBoolean(this string value)
        {
            return value == "True";
        }
    }
}
