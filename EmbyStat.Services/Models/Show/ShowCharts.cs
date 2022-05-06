﻿using System.Collections.Generic;
using EmbyStat.Services.Models.Charts;

namespace EmbyStat.Services.Models.Show;

public class ShowCharts
{
    public List<Chart> BarCharts { get; set; }
    public List<Chart> PieCharts { get; set; }

    public ShowCharts()
    {
        BarCharts = new List<Chart>();
        PieCharts = new List<Chart>();
    }
}