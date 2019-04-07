using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Graph
{
    public class DataSet<T>
    {
        public string Label { get; set; }
        public T[] Data { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public string HoverBackgroundColor { get; set; }
        public string HoverBorderColor { get; set; }
        public int BorderWidth { get; set; }

        public DataSet(int length, string label, Random rand)
        {
            Label = label;
            Data = new T[length];

            var red = rand.Next(0, 255);
            var green = rand.Next(0, 255);
            var blue = rand.Next(0, 255);

            BackgroundColor = GenerateRgbaString(red, green, blue, "0.5");
            BorderColor = GenerateRgbaString(red, green, blue, "0.8");
            HoverBackgroundColor = GenerateRgbaString(red, green, blue, "0.8");
            HoverBorderColor = GenerateRgbaString(red, green, blue, "1");
            BorderWidth = 1;
        }

        private string GenerateRgbaString(int red, int green, int blue, string alphaChannel)
        {
            return $"rgba({red},{green},{blue},{alphaChannel})";
        }
    }
}
