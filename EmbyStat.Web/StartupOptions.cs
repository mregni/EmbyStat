using CommandLine;

namespace EmbyStat.Web
{
    public class StartupOptions
    {
        [Option("port", Required = false, Default = 6555, HelpText = "Set the port EmbyStat needs to be hosted on")]
        public int Port { get; set; }
    }
}
