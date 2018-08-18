using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.Github.Models;

namespace EmbyStat.Services.Interfaces
{
    public interface IUpdateService
    {
        Task<CheckForUpdateResult> CheckForUpdate(CancellationToken cancellationToken);
        void UpdateServer();
    }
}
