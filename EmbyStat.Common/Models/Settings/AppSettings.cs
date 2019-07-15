namespace EmbyStat.Common.Models.Settings
{
    public class AppSettings
    {
        public string Version { get; set; }
        public string ProcessName { get; set; }
        public Updater Updater { get; set; }
        public Dirs Dirs { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public Rollbar Rollbar { get; set; }
        public string DatabaseFile { get; set; }
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
        public string Database { get; set; }
    }

    public class ConnectionStrings
    {
        public string Main { get; set; }
    }

    public class Rollbar
    {
        public string AccessToken { get; set; }
        public string Environment { get; set; }
        public string LogLevel { get; set; }
        public int MaxReportsPerMinute { get; set; }
        public int ReportingQueueDepth { get; set; }
    }
}
