using System;
using CommandLine;

namespace EmbyStat.Web
{
    public class StartupOptions
    {
        [Option("port", Required = false, Default = 6555, HelpText = "Set the port EmbyStat needs to be hosted on.")]
        public int Port { get; set; }

        [Option("no-updates", Required = false, Default = false, HelpText = "Disable all update flow and pages on the server.")]
        public bool NoUpdates { get; set; }
    }
}
