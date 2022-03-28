using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Common.SqLite;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories;

public class SqlJobRepository : IJobRepository
{
    private readonly DbContext _context;

    public SqlJobRepository(DbContext context)
    {
        _context = context;
    }

    public IEnumerable<SqlJob> GetAll()
    {
        return _context.Jobs.AsEnumerable();
    }

    public SqlJob GetById(Guid id)
    {
        return _context.Jobs.FirstOrDefault(x => x.Id == id);
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