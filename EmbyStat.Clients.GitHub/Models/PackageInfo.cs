using EmbyStat.Common.Enums;
using Newtonsoft.Json;

namespace EmbyStat.Clients.GitHub.Models;

public class PackageInfo
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("versionStr")]
    public string VersionStr { get; set; }
    [JsonProperty("classification")]
    public UpdateTrain Classification { get; set; }
    [JsonProperty("sourceUrl")]
    public string SourceUrl { get; set; }
    [JsonProperty("infoUrl")]
    public string InfoUrl { get; set; }
}