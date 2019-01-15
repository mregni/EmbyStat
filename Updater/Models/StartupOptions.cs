using CommandLine;

namespace Updater.Models
{
    public class StartupOptions
    {
        [Option("processId", Required = true)]
        public int ProcessId { get; set; }
        [Option("applicationPath", Required = true)]
        public string ApplicationPath { get; set; }
        [Option("processName", Required = true)]
        public string ProcessName { get; set; }
    }
}
