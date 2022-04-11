using System.Collections.Generic;
using Newtonsoft.Json;

namespace EmbyStat.Clients.GitHub.Models;

public class ReleaseObject
{
    [JsonProperty("tarball_url")]
    public string TarballUrl { get; set; }
    [JsonProperty("assets")]
    public List<Asset> Assets { get; set; }
    [JsonProperty("published_at")]
    public string PublishedAt { get; set; }
    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }
    [JsonProperty("prerelease")]
    public bool PreRelease { get; set; }
    [JsonProperty("author")]
    public Author Author { get; set; }
    [JsonProperty("draft")]
    public bool Draft { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("target_commitish")]
    public string TargetCommitish { get; set; }
    [JsonProperty("tag_name")]
    public string TagName { get; set; }
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("html_url")]
    public string HtmlUrl { get; set; }
    [JsonProperty("upload_url")]
    public string UploadUrl { get; set; }
    [JsonProperty("assets_url")]
    public string AssetsUrl { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("zipball_url")]
    public string ZipballUrl { get; set; }
    [JsonProperty("body")]
    public string Body { get; set; }
}