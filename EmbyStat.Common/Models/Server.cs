using System.Collections.Generic;

namespace EmbyStat.Common.Models
{
    public class Server
    {
        public bool CanSelfRestart { get; set; }
        public bool CanSelfUpdate { get; set; }
        public bool HasPendingRestart { get; set; }
        public bool HasUpdateAvailable { get; set; }
        public int HttpServerPortNumber { get; set; }
        public int HttpsPortNumber { get; set; }
        public string Id { get; set; }
        public string LocalAddress { get; set; }
        public string MacAddress { get; set; }
        public string OperatingSystem { get; set; }
        public string OperatingSystemDispayName { get; set; }
        public string ServerName { get; set; }
        public string SystemArchitecture { get; set; }
        public string SystemUpdateLevel { get; set; }
        public string Version { get; set; }
        public string WanAddress { get; set; }
        public int WebSocketPortNumber { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
