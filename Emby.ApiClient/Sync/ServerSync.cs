using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Session;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Data;
using Emby.ApiClient.Model;

namespace Emby.ApiClient.Sync
{
    public class ServerSync : IServerSync
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IFileTransferManager _fileTransferManager;
        private readonly ILogger _logger;
        private readonly ILocalAssetManager _localAssetManager;

        private readonly ClientCapabilities _clientCapabilities;
        private static readonly Dictionary<string, SemaphoreSlim> SemaphoreLocks = new Dictionary<string, SemaphoreSlim>(StringComparer.OrdinalIgnoreCase);

        public ServerSync(IConnectionManager connectionManager, ILogger logger, ILocalAssetManager localAssetManager, IFileTransferManager fileTransferManager, ClientCapabilities clientCapabilities)
        {
            _connectionManager = connectionManager;
            _fileTransferManager = fileTransferManager;
            _clientCapabilities = clientCapabilities;
            _logger = logger;
            _localAssetManager = localAssetManager;
        }

        public async Task Sync(ServerInfo server, bool enableCameraUpload, IProgress<double> progress, CancellationToken cancellationToken = default(CancellationToken))
        {
            var semaphore = GetLock(server.Id);

            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                await SyncInternal(server, enableCameraUpload, progress, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static SemaphoreSlim GetLock(string serverId)
        {
            SemaphoreSlim semaphore;

            if (SemaphoreLocks.TryGetValue(serverId, out semaphore))
            {
                return semaphore;
            }

            semaphore = new SemaphoreSlim(1, 1);
            SemaphoreLocks[serverId] = semaphore;

            return semaphore;
        }

        private async Task SyncInternal(ServerInfo server, bool enableCameraUpload, IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.Debug("Beginning ServerSync with server {0}, Id {1}", server.Name, server.Id);
            
            if (string.IsNullOrWhiteSpace(server.AccessToken) && string.IsNullOrWhiteSpace(server.ExchangeToken))
            {
                _logger.Info("Skipping sync process for server " + server.Name + ". No server authentication information available.");
                progress.Report(100);
                return;
            }

            // Don't need these here
            var result = await _connectionManager.Connect(server, new ConnectionOptions
            {
                EnableWebSocket = false,
                ReportCapabilities = false,
                UpdateDateLastAccessed = false

            }, cancellationToken).ConfigureAwait(false);

            if (result.State == ConnectionState.SignedIn)
            {
                await SyncInternal(server, result.ApiClient, enableCameraUpload, progress, cancellationToken).ConfigureAwait(false);
                progress.Report(100);
            }
            else
            {
                _logger.Info("Skipping sync process for server " + server.Name + ". ConnectionManager returned a state of {0}", result.State.ToString());
                progress.Report(100);
            }
        }

        private async Task SyncInternal(ServerInfo server, IApiClient apiClient, bool enableCameraUpload, IProgress<double> progress, CancellationToken cancellationToken)
        {
            const double cameraUploadTotalPercentage = .25;

            var uploadProgress = new DoubleProgress();
            uploadProgress.RegisterAction(p => progress.Report(p * cameraUploadTotalPercentage));

            if (enableCameraUpload)
            {
                await new ContentUploader(apiClient, _logger)
                    .UploadImages(uploadProgress, cancellationToken).ConfigureAwait(false);
            }

            var syncProgress = new DoubleProgress();
            syncProgress.RegisterAction(p => progress.Report((cameraUploadTotalPercentage * 100) + (p * (1 - cameraUploadTotalPercentage))));

            await new MediaSync(_localAssetManager, _logger, _fileTransferManager)
                .Sync(apiClient, server, uploadProgress, cancellationToken).ConfigureAwait(false);
        }
    }
}
