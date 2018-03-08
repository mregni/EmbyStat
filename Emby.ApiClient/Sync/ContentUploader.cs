using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Model;

namespace Emby.ApiClient.Sync
{
    public class ContentUploader
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger _logger;

        public ContentUploader(IApiClient apiClient, ILogger logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task UploadImages(IProgress<double> progress, CancellationToken cancellationToken = default(CancellationToken))
        {
            var device = _apiClient.Device;

            var deviceId = device.DeviceId;

            var history = await _apiClient.GetContentUploadHistory(deviceId).ConfigureAwait(false);

            var files = (await device.GetLocalPhotos().ConfigureAwait(false))
                .ToList();

            files.AddRange((await device.GetLocalVideos().ConfigureAwait(false)));

            files = files
                .Where(i => !history.FilesUploaded.Any(f => string.Equals(f.Id, i.Id, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var numComplete = 0;

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _logger.Debug("Uploading {0}", file.Id);

                try
                {
                    await device.UploadFile(file, _apiClient, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Error uploading file", ex);
                }

                numComplete++;
                double percent = numComplete;
                percent /= files.Count;

                progress.Report(100 * percent);
            }

            progress.Report(100);
        }
    }
}
