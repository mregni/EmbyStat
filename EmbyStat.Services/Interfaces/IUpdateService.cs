using System.Threading.Tasks;
using EmbyStat.Clients.GitHub.Models;
using EmbyStat.Common.Models.Settings;

namespace EmbyStat.Services.Interfaces
{
    public interface IUpdateService
    {
        UpdateResult CheckForUpdate();
        UpdateResult CheckForUpdate(UserSettings settings);
        Task DownloadZipAsync(UpdateResult result);
        Task UpdateServerAsync();
    }
}
