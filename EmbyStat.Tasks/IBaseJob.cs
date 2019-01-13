using System;
using System.Threading.Tasks;
using EmbyStat.Common.Hubs;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Tasks;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Repositories.Interfaces;
using Serilog;

namespace EmbyStat.Jobs
{
    public interface IBaseJob : IDisposable
    {
        Task Execute();
    }
}