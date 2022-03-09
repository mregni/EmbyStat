using System;

namespace EmbyStat.Common.SqLite.Users;

public class SqlUser
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
    public SqlUserConfiguration Configuration { get; set; }
    public Guid PolicyId { get; set; }
    public SqlUserPolicy Policy { get; set; }
}
