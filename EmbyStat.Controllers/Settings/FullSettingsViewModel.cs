using System;
using System.Collections.Generic;

namespace EmbyStat.Controllers.Settings
{
    public class FullSettingsViewModel
    {
        public Guid? Id { get; set; }
        public string AppName { get; set; }
        public bool WizardFinished { get; set; }
        public string Username { get; set; }
        public string Language { get; set; }
        public int ToShortMovie { get; set; }
        public bool ToShortMovieEnabled { get; set; }
        public int KeepLogsCount { get; set; }
        public List<int> MovieLibraryTypes { get; set; }
        public List<int> ShowLibraryTypes { get; set; }
        public bool AutoUpdate { get; set; }
        public int UpdateTrain { get; set; }
        public bool UpdateInProgress { get; set; }
        public EmbySettingsViewModel Emby { get; set; }
        public TvdbSettingsViewModel Tvdb { get; set; }
        public string Version { get; set; }
        public bool NoUpdates { get; set; }
        public bool EnableRollbarLogging { get; set; }
        public string DataDir { get; set; }
        public string LogDir { get; set; }
        public string ConfigDir { get; set; }
        
        public class EmbySettingsViewModel
        {
            public string ServerName { get; set; }
            public string ServerAddress { get; set; }
            public string ApiKey { get; set; }
            public int ServerPort { get; set; }
            public string AuthorizationScheme { get; set; }
            public int ServerProtocol { get; set; }
        }

        public class TvdbSettingsViewModel
        {
            public DateTime? LastUpdate { get; set; }
            public string ApiKey { get; set; }
        }
    }
}
