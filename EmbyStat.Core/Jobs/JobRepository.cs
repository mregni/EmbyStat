﻿using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Core.DataStore;
using EmbyStat.Core.Jobs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Core.Jobs;

public class JobRepository : IJobRepository
{
    private readonly EsDbContext _context;

    public JobRepository(EsDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Job> GetAll()
    {
        return _context.Jobs.AsNoTracking().AsEnumerable();
    }

    public Job? GetById(Guid id)
    {
        return _context.Jobs.AsNoTracking().FirstOrDefault(x => x.Id == id);
    }

    public async Task StartJob(Guid id)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(x => x.Id == id);
        if (job != null)
        {
            job.CurrentProgressPercentage = 0;
            job.State = JobState.Running;
            job.StartTimeUtc = DateTime.UtcNow;
            job.EndTimeUtc = null;
            await _context.SaveChangesAsync();
        }
    }

    public async Task EndJob(Guid id, DateTime endTime, JobState state)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(x => x.Id == id);
        if (job != null)
        {
            job.CurrentProgressPercentage = 100;
            job.State = state;
            job.EndTimeUtc = endTime;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UpdateTrigger(Guid id, string trigger)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(x => x.Id == id);
        if (job != null)
        {
            job.Trigger = trigger;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task ResetAllJobs()
    {
        var jobs = _context.Jobs.Where(job => job.State == JobState.Running);

        foreach (var job in jobs)
        {
            job.State = JobState.Failed;
            job.EndTimeUtc = DateTime.Now;
        }

        await _context.SaveChangesAsync();
    }
}