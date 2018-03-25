using Emby.ApiClient;
using EmbyStat.Services.Helpers;
using MediaBrowser.Model.Logging;

namespace EmbyStat.Services.EmbyClientFacade
{
    public static class Client
    {
	    public static ApiClient GetApiClient(string embyAddress)
	    {
		    var logger = new NullLogger();

		    var device = new Device
		    {
			    DeviceName = Constants.DeviceName,
			    DeviceId = Constants.DeviceId
		    };

		    return new ApiClient(logger, embyAddress, Constants.AppName, device, Constants.AppVersion, new CryptographyProvider());
	    }
	}
}
