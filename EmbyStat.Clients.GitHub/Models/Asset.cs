using Newtonsoft.Json;

namespace EmbyStat.Clients.GitHub.Models
{
    public class Asset
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("label")]
        public object Label { get; set; }
        [JsonProperty("uploader")]
        public Author Uploader { get; set; }
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("download_count")]
        public int DownloadCount { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
        [JsonProperty("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
    }
}
