using System;

namespace EmbyStat.Common.SqLite;

public class SqlDevice
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string LastUserName { get; set; }
    public string AppName { get; set; }
    public string AppVersion { get; set; }
    public string LastUserId { get; set; }
    public DateTime? DateLastActivity { get; set; }
    public string IconUrl { get; set; }
}