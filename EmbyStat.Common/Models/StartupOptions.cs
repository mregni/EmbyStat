using CommandLine;

namespace EmbyStat.Common.Models
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

        [Option("log-level", Required = false, Default = 2, HelpText = "Set the proper log level\n1: Debug\n2: Information")]
        public int LogLevel { get; set; }

        [Option("listen-urls", Required = false, Default = "http://*", HelpText = "Set the url's where EmbyStat needs to listen on. Default is http://* but you can change it to only loopback address here if needed. Don't add port number, it will be added automatically. Use ; if you want to define more then one url.")]
        public string ListeningUrls { get; set; }
    }
} 
