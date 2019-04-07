using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Graph
{
    public class BarGraph<T>
    {
        public string Title { get; set; }
        public string[] Labels { get; set; }
        public List<DataSet<T>> DataSets { get; set; }

        private int Length { get; set; }

        public BarGraph<T> InitiateForHourOfDayGraph(string title)
        {
            Title = title;
            Length = 24;
            Labels = new string[Length];
            DataSets = new List<DataSet<T>>();
            for (var i = 0; i < Length; i++)
            {
                Labels[i] = new DateTime(0).AddHours(i).ToString("HH:mm");
            }

            return this;
        }

        public int GetLength()
        {
            return Length;
        }
    }

    public class DataSet<T>
    {
        public string Label { get; set; }
        public T[] Data { get; set; }

        public DataSet(int length, string label)
        {
            Label = label;
            Data = new T[length];
        }
    }
}
