using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Emby
{
    public class ServerInfoViewModel
    {
	    public bool SupportsAutoRunAtStartup { get; set; }
	    public bool HasUpdateAvailable { get; set; }
	    public int HttpsPortNumber { get; set; }
	    public bool SupportsHttps { get; set; }
	    public int HttpServerPortNumber { get; set; }
	    public string TranscodingTempPath { get; set; }
	    public string InternalMetadataPath { get; set; }
	    public string LogPath { get; set; }
	    public string CachePath { get; set; }
	    public string ItemsByNamePath { get; set; }
	    public string ProgramDataPath { get; set; }
	    public string EncoderLocationType { get; set; }
	    public bool CanSelfUpdate { get; set; }
	    public bool CanSelfRestart { get; set; }
	    public int WebSocketPortNumber { get; set; }
	    public bool SupportsLibraryMonitor { get; set; }
	    public bool IsShuttingDown { get; set; }
	    public bool HasPendingRestart { get; set; }
	    public string PackageName { get; set; }
	    public string OperatingSystemDisplayName { get; set; }
	    public int SystemUpdateLevel { get; set; }
	    public bool CanLaunchWebBrowser { get; set; }
	    public int SystemArchitecture { get; set; }
	    public List<DriveViewModel> Drives { get; set; }
	}
}
