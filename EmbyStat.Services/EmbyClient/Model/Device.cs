using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Devices;

namespace EmbyStat.Services.EmbyClient.Model
{
    public class Device : IDevice
    {
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
    }
}
