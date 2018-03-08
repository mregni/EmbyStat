using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Sync;
using System;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Data;
using Emby.ApiClient.Model;

namespace Emby.ApiClient.Sync
{
    class FileTransferManager : IFileTransferManager
    {
        private readonly ILocalAssetManager _localAssetManager;
        private readonly ILogger _logger;

        internal FileTransferManager(ILocalAssetManager localAssetManager, ILogger logger)
        {
            _localAssetManager = localAssetManager;
            _logger = logger;
        }

        public async Task GetItemFileAsync(IApiClient apiClient,
                                      ServerInfo server,
                                      LocalItem item,
                                      string syncJobItemId,
                                      IProgress<double> transferProgress,
                                      CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.Debug("Downloading media with Id {0} to local repository", item.Item.Id);

            using (var stream = await apiClient.GetSyncJobItemFile(syncJobItemId, cancellationToken).ConfigureAwait(false))
            {
                await _localAssetManager.SaveMedia(stream, item, server).ConfigureAwait(false);
            }
            transferProgress.Report(100);
        }
    }
}
