using MediaBrowser.Model.ApiClient;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.ApiClient
{
    public interface IServerLocator
    {
        /// <summary>
        /// Attemps to discover the server within a local network
        /// </summary>
        Task<List<ServerDiscoveryInfo>> FindServers(int timeoutMs, CancellationToken cancellationToken = default(CancellationToken));
    }
}