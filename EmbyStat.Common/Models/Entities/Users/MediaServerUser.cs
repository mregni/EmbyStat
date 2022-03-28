using System;

namespace EmbyStat.Common.Models.Entities.Users;

public class MediaServerUser
{
    public string Name { get; set; }
    public string ServerId { get; set; }
    public string Id { get; set; }
    public bool HasPassword { get; set; }
    public bool HasConfiguredPassword { get; set; }
    public bool HasConfiguredEasyPassword { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
    public DateTimeOffset? LastActivityDate { get; set; }
    public Guid ConfigurationId { get; set; }
    public MediaServerUserConfiguration Configuration { get; set; }
    public Guid PolicyId { get; set; }
    public MediaServerUserPolicy Policy { get; set; }
}
