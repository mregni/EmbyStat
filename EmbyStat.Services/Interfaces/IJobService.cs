using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces;

public interface IJobService
{
    IEnumerable<Job> GetAll();
    Job GetById(Guid id);
    Task<bool> UpdateTrigger(Guid id, string trigger);
    Task ResetAllJobs();
}