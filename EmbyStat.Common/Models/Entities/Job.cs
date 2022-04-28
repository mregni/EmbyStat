using System;
using EmbyStat.Common.Models.Tasks.Enum;

namespace EmbyStat.Common.Models.Entities;

public class Job
{
    public Guid Id { get; set; }
    public JobState State { get; set; }
    public double? CurrentProgressPercentage { get; set; }
    public DateTime? StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Trigger { get; set; }

    /// <summary>
    /// Constructor for EF
    /// </summary>
    public Job()
    {
    }

    /// <summary>
    /// Constructor for Job seeder
    /// </summary>
    /// <param name="id">Job id</param>
    /// <param name="title">Job title</param>
    /// <param name="trigger">Job trigger string (cron string)</param>
    public Job(Guid id, string title, string trigger)
    {
        Id = id;
        Trigger = trigger;
        Title = title;
        Description = $"{Title}DESCRIPTION";
        State = JobState.Idle;
        CurrentProgressPercentage = 0;
        EndTimeUtc = null;
        StartTimeUtc = null;
    }
}