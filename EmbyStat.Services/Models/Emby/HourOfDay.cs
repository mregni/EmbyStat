using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Emby
{
    public class HourOfDay
    {
        public string Label { get; set; }
        public int Value { get; set; }

        public HourOfDay(string label, int value)
        {
            Label = label;
            Value = value;
        }
    }
}
