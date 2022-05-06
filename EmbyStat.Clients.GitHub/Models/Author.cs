using Newtonsoft.Json;

namespace EmbyStat.Clients.GitHub.Models;

public class Author
{
    [JsonProperty("received_events_url")]
    public string ReceivedEventsUrl { get; set; }
    [JsonProperty("events_url")]
    public string EventsUrl { get; set; }
    [JsonProperty("repos_url")]
    public string ReposUrl { get; set; }
    [JsonProperty("organizations_url")]
    public string OrganizationsUrl { get; set; }
    [JsonProperty("subscriptions_url")]
    public string SubscriptionsUrl { get; set; }
    [JsonProperty("starred_url")]
    public string StarredUrl { get; set; }
    [JsonProperty("gists_url")]
    public string GistsUrl { get; set; }
    [JsonProperty("following_url")]
    public string FollowingUrl { get; set; }
    [JsonProperty("followers_url")]
    public string FollowersUrl { get; set; }
    [JsonProperty("html_url")]
    public string HtmlUrl { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("gravatar_id")]
    public string GravatarId { get; set; }
    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; }
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("login")]
    public string Login { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("site_admin")]
    public bool SiteAdmin { get; set; }
}