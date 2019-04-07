using System;
using System.Collections.Generic;
using EmbyStat.Services.Models.Graph;

namespace EmbyStat.Controllers.HelperClasses.Graphs
{
    public class BarGraphViewModel<T>
    {
        public string Title { get; set; }
        public string[] Labels { get; set; }
        // ReSharper disable once IdentifierTypo
        //Chart.js needs array of Datasets not DataSets, that's why there is a typo here!
        public List<DataSetViewModel<T>> Datasets { get; set; }
    }
}
