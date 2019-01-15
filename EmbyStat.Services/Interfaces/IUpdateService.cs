using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.Github.Models;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Services.Interfaces
{
    public interface IUpdateService
    {
        Task<UpdateResult> CheckForUpdate(CancellationToken cancellationToken);
        Task<UpdateResult> CheckForUpdate(Configuration settings, CancellationToken cancellationToken);
        Task DownloadZip(UpdateResult result);
        Task UpdateServer();
    }
}
