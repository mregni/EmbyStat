using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Model;

namespace Emby.ApiClient
{
    public class Device : IDevice
    {
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }

        public virtual async Task<IEnumerable<LocalFileInfo>> GetLocalPhotos()
        {
            return new List<LocalFileInfo>();
        }

        public virtual async Task<IEnumerable<LocalFileInfo>> GetLocalVideos()
        {
            return new List<LocalFileInfo>();
        }

        public Task UploadFile(LocalFileInfo file, IApiClient apiClient, CancellationToken cancellationToken = default(CancellationToken))
        {
            return apiClient.UploadFile(File.OpenRead(file.Id), file, cancellationToken);
        }
    }
}
