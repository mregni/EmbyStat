using System;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Model;
using MediaBrowser.Model.ApiClient;

namespace Emby.ApiClient.Sync
{
    public interface IMediaSync
    {
        Task Sync(IApiClient apiClient,
            ServerInfo serverInfo,
            IProgress<double> progress,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}