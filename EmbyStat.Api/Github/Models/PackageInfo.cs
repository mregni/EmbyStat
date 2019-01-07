using EmbyStat.Common.Enums;

namespace EmbyStat.Api.Github.Models
{
    public class PackageInfo
    {
        public string name { get; set; }
        public string versionStr { get; set; }
        public UpdateTrain classification { get; set; }
        public string sourceUrl { get; set; }
        public string infoUrl { get; set; }
    }
}
