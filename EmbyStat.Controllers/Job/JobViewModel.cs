﻿using System;

namespace EmbyStat.Controllers.Job;

public class JobViewModel
{
    public int State { get; set; }
    public double? CurrentProgressPercentage { get; set; }
    public Guid Id { get; set; }
    public string StartTimeUtcIso { get; set; }
    public string EndTimeUtcIso { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Trigger { get; set; }
}