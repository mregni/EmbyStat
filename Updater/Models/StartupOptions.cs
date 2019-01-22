using CommandLine;

namespace Updater.Models
{
    public class StartupOptions
    {
        [Option("processId", Required = true, HelpText = "Process id of the current running EmbyStat process")]
        public int ProcessId { get; set; }
        [Option("applicationPath", Required = true, HelpText = "Path where EmbyStat is installed")]
        public string ApplicationPath { get; set; }
        [Option("processName", Required = true, HelpText = "Process name of the current EmbyStat process")]
        public string ProcessName { get; set; }
        [Option("port", Required = true, Default = 5432, HelpText = "Port number EmbyStat is running on")]
        public int Port { get; set; }
    }
}
