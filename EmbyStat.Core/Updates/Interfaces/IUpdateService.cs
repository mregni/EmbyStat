using EmbyStat.Clients.GitHub.Models;

namespace EmbyStat.Core.Updates.Interfaces;

public interface IUpdateService
{
    Task<UpdateResult> CheckForUpdate();
    Task DownloadZipAsync(UpdateResult result);
    Task UpdateServerAsync();
}