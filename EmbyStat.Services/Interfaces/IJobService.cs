using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.SqLite;

namespace EmbyStat.Services.Interfaces
{
    public interface IJobService
    {
        IEnumerable<SqlJob> GetAll();
        SqlJob GetById(Guid id);
        Task<bool> UpdateTrigger(Guid id, string trigger);
        Task ResetAllJobs();
    }
}
