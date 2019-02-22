namespace EmbyStat.Common.Models.Settings
{
    public class AppSettings
    {
        public string Version { get; set; }
        public string ProcessName { get; set; }
        public Updater Updater { get; set; }
        public Dirs Dirs { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        /// <summary>
        /// Port number, is set dynamically when server is starting
        /// </summary>
        public int Port { get; set; }
    }

    public class Updater
    {
        public string UpdateAsset { get; set; }
        public string Dll { get; set; }
        public string GithubUrl { get; set; }
        public string DevString { get; set; }
        public string BetaString { get; set; }
    }

    public class Dirs
    {
        public string TempUpdateDir { get; set; }
        public string Updater { get; set; }
        public string Logs { get; set; }
        public string Settings { get; set; }
    }

    public class ConnectionStrings
    {
        public string Main { get; set; }
    }
}
