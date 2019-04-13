using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Models.Settings;

namespace EmbyStat.Services.Interfaces
{
    public interface IUpdateService
    {
        Task<UpdateResult> CheckForUpdateAsync(CancellationToken cancellationToken);
        Task<UpdateResult> CheckForUpdateAsync(UserSettings settings, CancellationToken cancellationToken);
        Task DownloadZipAsync(UpdateResult result);
        Task UpdateServerAsync();
    }
}
