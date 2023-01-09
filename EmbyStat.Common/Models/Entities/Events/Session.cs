using System;
using System.Collections.Generic;

namespace EmbyStat.Common.Models.Entities.Events;

public class Session
{
    public string Id { get; set; }
    public string ServerId { get; set; }
    public string UserId { get; set; }
    public string Client { get; set; }
    public string DeviceId { get; set; }
    public string ApplicationVersion { get; set; }
    public string AppIconUrl { get; set; }
    public string RemoteEndPoint { get; set; }
    public string Protocol { get; set; }
    public DateTime LastActivityDate { get; set; }
    public ICollection<MediaPlay> MediaPlays { get; set; }
}