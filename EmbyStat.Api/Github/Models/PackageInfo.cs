using System;
using System.Runtime.Serialization;
using EmbyStat.Common.Enums;

namespace EmbyStat.Api.Github.Models
{
    public class PackageInfo
    {
        public string name { get; set; }
        public string guid { get; set; }
        public string versionStr { get; set; }
        [IgnoreDataMember]
        public Version version { get; }
        public UpdateLevel classification { get; set; }
        public string description { get; set; }
        public string requiredVersionStr { get; set; }
        public string sourceUrl { get; set; }
        public string checksum { get; set; }
        public string targetFilename { get; set; }
        public string infoUrl { get; set; }
        public string runtimes { get; set; }
    }
}
