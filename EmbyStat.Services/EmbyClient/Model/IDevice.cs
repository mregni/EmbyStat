using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Devices;

namespace EmbyStat.Services.EmbyClient.Model
{
    public interface IDevice
    {
	    string DeviceName { get; }
	    string DeviceId { get; }
	}
}
