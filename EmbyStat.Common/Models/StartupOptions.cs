using CommandLine;

namespace EmbyStat.Web
{
    public class StartupOptions
    {
        [Option("port", Required = false, Default = 6555, HelpText = "Set the port EmbyStat needs to be hosted on.")]
        public int Port { get; set; }

        [Option("no-updates", Required = false, Default = false, HelpText = "Disable all update flow and pages on the server.")]
        public bool NoUpdates { get; set; }

        [Option("data-dir", Required = false, Default = "", HelpText = "Folder where database is stored")]
        public string DataDir { get; set; }

        [Option("log-dir", Required = false, Default = "", HelpText = "Folder where log files are stored")] 
        public string LogDir { get; set; }

        [Option("config-dir", Required = false, Default = "", HelpText = "Folder where config files are stored")]
        public string ConfigDir { get; set; }

        [Option("service", Required = false, Default = false, HelpText = "Indicate EmbyStat is running as a service")]
        public bool Service { get; set; }
    }
} 
