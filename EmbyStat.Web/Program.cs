using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;

namespace Web
{
    public class Program
    {
		public static void Main(string[] args)
	    {
		    var host = BuildWebHost(args);

#if !DEBUG
	        OpenBrowser("http://localhost:5123");
#endif

			host.Run();
	    }

	    public static IWebHost BuildWebHost(string[] args) =>
		    WebHost.CreateDefaultBuilder(args)
			    .UseStartup<Startup>()
			    .UseNLog()
				.Build();

	    public static void OpenBrowser(string url)
	    {
		    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		    {
			    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
		    }
		    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		    {
			    Process.Start("open", url);
		    }
		    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		    {
			    Process.Start("xdg-open", url);
		    }
	    }
	}
}

